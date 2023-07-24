using Elevate.Interface.HumanAPI;
using Elevate.Model.HumanAPI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Elevate.Utilities.Errors;
using Elevate.Interface.Identity;
using System.Net.Http.Json;

namespace Elevate.Service.HumanAPI
{
    public class HumanAPIService : IHumanAPIService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<HumanAPIService> _logger;
        private readonly IUserManagerService _userManagerService;
        private readonly IHumanAPIRepository _humanAPIRepository;

        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _accessTokenEndpoint;
        private readonly string _publicTokenEndpoint;

        public HumanAPIService(
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            ILogger<HumanAPIService> logger,
            IUserManagerService userManagerService,
            IHumanAPIRepository humanAPIRepository)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
            _logger = logger;
            _userManagerService = userManagerService;
            _humanAPIRepository = humanAPIRepository;

            _clientId = _config["HumanAPI:Client_Id"];
            _clientSecret = _config["HumanAPI:Client_Secret"];
            _accessTokenEndpoint = _config["HumanAPI:AccessTokenEndpoint"];
            _publicTokenEndpoint = _config["HumanAPI:PublicTokenEndpoint"];

            if (string.IsNullOrEmpty(_clientId) || string.IsNullOrEmpty(_clientSecret) ||
                string.IsNullOrEmpty(_accessTokenEndpoint) || string.IsNullOrEmpty(_publicTokenEndpoint))
            {
                throw new ApplicationException("HumanAPI configuration is incomplete.");
            }
        }

        public async Task<string> GetUserAccessToken(SessionTokenObject accessTokenRequest)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();

                var httpResponse = await httpClient.PostAsJsonAsync(_accessTokenEndpoint, accessTokenRequest);

                if (httpResponse.IsSuccessStatusCode)
                {
                    var result = await httpResponse.Content.ReadFromJsonAsync<AccessTokenResponse>();
                    await UpdateHumanAPIUser(result);
                    return result.AccessToken;
                }
                else
                {
                    var errorMessage = await httpResponse.Content.ReadAsStringAsync();
                    throw new APIException((int)httpResponse.StatusCode, $"API request failed: {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<string> RequestPublicToken(string email)
        {
            try
            {
                var clientUserId = (await _userManagerService.FindByEmailAsync(email))?.Id;
                if (string.IsNullOrEmpty(clientUserId))
                {
                    throw new InvalidOperationException("User not found.");
                }

                var humanUser = await _humanAPIRepository.GetHumanAPIUser(clientUserId);
                return humanUser?.PublicToken ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<string> RequestSessionToken(string email)
        {
            try
            {
                var clientUserId = (await _userManagerService.FindByEmailAsync(email))?.Id;
                if (string.IsNullOrEmpty(clientUserId))
                {
                    throw new InvalidOperationException("User not found.");
                }

                var httpClient = _httpClientFactory.CreateClient();
                var requestContent = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "client_id", _clientId },
                    { "client_secret", _clientSecret },
                    { "client_user_id", clientUserId },
                    { "client_user_email", email },
                    { "type", "session" }
                });

                var authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);

                using var response = await httpClient.PostAsync(_accessTokenEndpoint, requestContent);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<SessionTokenResponse>();
                    await UpdateHumanAPIUserHumanID(clientUserId, result.Human_id);
                    return result?.Session_token ?? string.Empty;
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    throw new APIException((int)response.StatusCode, $"API request failed: {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        private async Task<HumanAPIUser> UpdateHumanAPIUserHumanID(string userId, string human_id)
        {
            var humanUser = await _humanAPIRepository.GetHumanAPIUser(userId);
            if (humanUser != null)
            {
                humanUser.HumanID = human_id;
                return await _humanAPIRepository.UpdateHumanAPIUser(humanUser);
            }
            return null;
        }

        private async Task<HumanAPIUser> UpdateHumanAPIUser(AccessTokenResponse accessTokenResponse)
        {
            var user = await _userManagerService.FindByEmailAsync(accessTokenResponse.ClientUserId);
            if (user != null)
            {
                var humanUser = await _humanAPIRepository.GetHumanAPIUser(user.Id);
                if (humanUser!=null && string.IsNullOrEmpty(humanUser?.AccessToken))
                {
                    humanUser.AccessToken = accessTokenResponse.AccessToken;
                    humanUser.PublicToken = accessTokenResponse.PublicToken;
                    humanUser.HumanID = accessTokenResponse.HumanId;
                    return await _humanAPIRepository.UpdateHumanAPIUser(humanUser);
                }
                else
                {
                    return humanUser;
                }
            }
            return null;
        }
    }
}