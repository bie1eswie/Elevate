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
        private readonly string _clientId = string.Empty;
        private readonly string _clientSecret = string.Empty;
        private readonly string _accessTokenEndpoint = string.Empty;
        private readonly string _publicTokenEndpoint = string.Empty;

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
        }

        public async Task<string> RequestPublicToken(string email)
        {
            try
            {
                var clientUserId = (await _userManagerService.FindByEmailAsync(email)).Id;
                var humanId = (await _humanAPIRepository.GetHumanAPIUser(clientUserId)).HumanID;

                var httpClient = _httpClientFactory.CreateClient();
                var httpResponse = await httpClient.PostAsJsonAsync(_publicTokenEndpoint, new PublicTokenRequest { clientId = _clientId, clientSecret = _clientSecret, humanId = humanId });

                if (httpResponse.IsSuccessStatusCode)
                {
                    var result = await httpResponse.Content.ReadFromJsonAsync<PublicTokenResponse>();
                    await UpdateHumanAPIUserPublicToken(clientUserId, result.publicToken);
                    return result.publicToken;
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

        public async Task<string> RequestToken(string email)
        {
            try
            {
                var clientUserId = (await _userManagerService.FindByEmailAsync(email)).Id;

                var httpClient = _httpClientFactory.CreateClient();
                var requestContent = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "client_id", _clientId },
                    { "client_secret", _clientSecret },
                    { "client_user_id", clientUserId },
                    { "client_user_email", email },
                    { "Content-Type", "application/json" },
                    { "type", "session" }
                });

                var authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);

                using var response = await httpClient.PostAsync(_accessTokenEndpoint, requestContent);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<AccessTokenResponse>();
                    await UpdateHumanAPIUserHumanID(clientUserId, result.human_id);
                    return result?.session_token ?? String.Empty;
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
            humanUser.HumanID = human_id;
            return await _humanAPIRepository.UpdateHumanAPIUser(humanUser);
        }

        private async Task<HumanAPIUser> UpdateHumanAPIUserPublicToken(string userId, string publicToken)
        {
            var humanUser = await _humanAPIRepository.GetHumanAPIUser(userId);
            if (string.IsNullOrEmpty(humanUser.PublicToken))
            {
                humanUser.PublicToken = publicToken;
                return await _humanAPIRepository.UpdateHumanAPIUser(humanUser);
            }
            else
            {
                return humanUser;
            }
        }
    }
}
