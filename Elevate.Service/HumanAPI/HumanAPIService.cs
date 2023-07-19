using Elevate.Interface.HumanAPI;
using Elevate.Model.HumanAPI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Json;
using Elevate.Utilities.Errors;
using Elevate.Interface.Identity;

namespace Elevate.Service.HumanAPI
{
    public class HumanAPIService : IHumanAPIService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<HumanAPIService> _logger;
        private readonly IUserManagerService _userManagerService;
        public HumanAPIService(IHttpClientFactory httpClientFactory, IConfiguration config, ILogger<HumanAPIService> logger, IUserManagerService userManagerService)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
            _logger = logger;
            _userManagerService = userManagerService;
        }
        public async Task<string> RequestToken(string email)
        {
            try
            {
                var clientId = _config["HumanAPI:Client_Id"];
                var clientSecret = _config["HumanAPI:Client_Secret"];
                var clientUserId = Guid.NewGuid().ToString();
                using var httpClient = _httpClientFactory.CreateClient();
                var requestContent = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "client_id", clientId },
                    { "client_secret", clientSecret },
                    { "client_user_id", clientUserId },
                    { "client_user_email", email },
                    { "Content-Type", "application/json" },
                    { "type", "session" }
                });

                 var authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
                 httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);

                using var response = await httpClient.PostAsync(_config["HumanAPI:AccessTokenEndpoint"], requestContent);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<AccessTokenResponse>();
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
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
