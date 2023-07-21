using Elevate.Interface.HumanAPI;
using Elevate.Model.Data;
using Elevate.Utilities.Helpers;
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
using Elevate.Model.HumanAPI;
using Elevate.Interface.Identity;
using System.Net.Http.Json;

namespace Elevate.Service.HumanAPI
{
    public class HumanDataAPIService : IHumanDataAPIService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<HumanDataAPIService> _logger;
        private readonly IUserManagerService _userManagerService;
        private readonly IHumanAPIRepository _humanAPIRepository;
        private readonly string _clientTokenEndpoint = string.Empty;
        private readonly string _dataAPIBase = string.Empty;

        public HumanDataAPIService(
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            ILogger<HumanDataAPIService> logger,
            IUserManagerService userManagerService,
            IHumanAPIRepository humanAPIRepository)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
            _logger = logger;
            _userManagerService = userManagerService;
            _humanAPIRepository = humanAPIRepository;
            _clientTokenEndpoint = _config["HumanAPI:DataAPI:clientTokenEndpoint"];
            _dataAPIBase = _config["HumanAPI:DataAPI:apiBase"];
        }

        public async Task<IReadOnlyList<ActivitySummary>> GetActivitySummary(string accessToken)
        {
            try
            {
                using var httpClient = CreateAuthorizedHttpClient(accessToken);

                var endpoint = _dataAPIBase + Constants.GET_ACTIVITY_SUMMARY;
                var httpResponse = await httpClient.GetAsync(endpoint);

                if (httpResponse.IsSuccessStatusCode)
                {
                    var result = await httpResponse.Content.ReadFromJsonAsync<IReadOnlyList<ActivitySummary>>();
                    return result;
                }
                else
                {
                    await HandleUnsuccessfulResponse(httpResponse);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<IReadOnlyList<HeartRateReading>> GetVitals(string accessToken, string vitalName)
        {
            try
            {
                using var httpClient = CreateAuthorizedHttpClient(accessToken);

                var endpoint = _dataAPIBase + Constants.VitalsDataEndPoints[vitalName];
                var httpResponse = await httpClient.GetAsync(endpoint);

                if (httpResponse.IsSuccessStatusCode)
                {
                    var result = await httpResponse.Content.ReadFromJsonAsync<IReadOnlyList<HeartRateReading>>();
                    return result;
                }
                else
                {
                    await HandleUnsuccessfulResponse(httpResponse);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<string> ResyncData(string email)
        {
            try
            {
                var user = await _userManagerService.FindByEmailAsync(email);
                var humanAPIUser = await _humanAPIRepository.GetHumanAPIUser(user.Id);
                var clientToken = await GetClientToken();

                var currentSyncStatus = await GetLatestSyncStatus(clientToken);
                if (currentSyncStatus == null)
                {
                    throw new APIException(500, "API request failed:");
                }

                using var httpClient = CreateAuthorizedHttpClient(clientToken);

                var endpoint = _dataAPIBase + humanAPIUser.HumanID + "/actions";
                var httpResponse = await httpClient.PostAsJsonAsync(endpoint, new UserActionRequest() { AccountId = currentSyncStatus.Id, Action = "resync" });

                if (httpResponse.IsSuccessStatusCode)
                {
                    var result = await httpResponse.Content.ReadAsStringAsync();
                    return result;
                }
                else
                {
                    await HandleUnsuccessfulResponse(httpResponse);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        private HttpClient CreateAuthorizedHttpClient(string accessToken)
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return httpClient;
        }

        private async Task HandleUnsuccessfulResponse(HttpResponseMessage httpResponse)
        {
            var errorMessage = await httpResponse.Content.ReadAsStringAsync();
            throw new APIException((int)httpResponse.StatusCode, $"API request failed: {errorMessage}");
        }

        private async Task<SyncStatusResponse> GetLatestSyncStatus(string clientToken)
        {
            try
            {
                using var httpClient = CreateAuthorizedHttpClient(clientToken);

                var endpoint = _dataAPIBase + Constants.GET_ACCOUNT_ID;
                var httpResponse = await httpClient.GetAsync(endpoint);
                if (httpResponse.IsSuccessStatusCode)
                {
                    return await httpResponse.Content.ReadFromJsonAsync<SyncStatusResponse>();
                }
                else
                {
                    await HandleUnsuccessfulResponse(httpResponse);
                    return null; // This line will never be reached due to the exception throw above
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        private async Task<string> GetClientToken()
        {
            try
            {
                var clientId = _config["HumanAPI:Client_Id"];
                var clientSecret = _config["HumanAPI:Client_Secret"];

                using var httpClient = _httpClientFactory.CreateClient();
                var requestContent = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "client_id", clientId },
                    { "client_secret", clientSecret },
                    { "Content-Type", "application/json" },
                    { "type", "client" }
                });

                var authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);

                using var httpResponse = await httpClient.PostAsync(_clientTokenEndpoint, requestContent);

                if (httpResponse.IsSuccessStatusCode)
                {
                    return await httpResponse.Content.ReadAsStringAsync();
                }
                else
                {
                    await HandleUnsuccessfulResponse(httpResponse);
                    return null; // This line will never be reached due to the exception throw above
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
