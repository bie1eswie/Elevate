using Elevate.Interface.HumanAPI;
using Elevate.Model.Data;
using Elevate.Utilities.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using Elevate.Utilities.Errors;
using Elevate.Model.HumanAPI;

namespace Elevate.Service.HumanAPI
{
    public class HumanDataAPIService : IHumanDataAPIService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<HumanDataAPIService> _logger;
        public HumanDataAPIService(IHttpClientFactory httpClientFactory, IConfiguration config, ILogger<HumanDataAPIService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
            _logger = logger;
        }

        public async Task<IReadOnlyList<ActivitySummary>> GetActivitySummary(string accessToken)
        {
            try
            {
                var basePath = _config["HumanAPI:DataAPI:apiBase"];
                using var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var endpoint = basePath + Constants.GET_ACTIVITY_SUMMARY;
                var httpResponse = await httpClient.GetAsync(endpoint);

                if (httpResponse.IsSuccessStatusCode)
                {
                    var result = await httpResponse.Content.ReadFromJsonAsync<IReadOnlyList<ActivitySummary>>();
                    return result;
                }
                else
                {
                    var errorMessage = await httpResponse.Content.ReadAsStringAsync();
                    throw new APIException((int)httpResponse.StatusCode, $"API request failed: {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<IReadOnlyList<HeartRateReading>> GetVitals(string accessToken, string vitalName)
        {
            try
            {
                var basePath = _config["HumanAPI:DataAPI:apiBase"];

                using var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var endpoint = basePath + Constants.VitalsDataEndPoints[vitalName];
                var httpResponse = await httpClient.GetAsync(endpoint);

                if (httpResponse.IsSuccessStatusCode)
                {
                    var result = await httpResponse.Content.ReadFromJsonAsync<IReadOnlyList<HeartRateReading>>();
                    return result;
                }
                else
                {
                    var errorMessage = await httpResponse.Content.ReadAsStringAsync();
                    throw new APIException((int)httpResponse.StatusCode, $"API request failed: {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task ResyncData(string accessToken,string human_id)
        {
            try
            {
                var currentSyncStatus = await this.getLatestSyncStatus(accessToken);
                if (currentSyncStatus == null)
                {
                    throw new APIException(500, "API request failed:");
                }

                var basePath = _config["HumanAPI:UsersAPIBase"];

                using var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var endpoint = basePath + human_id+ "/actions";
                var httpResponse = await httpClient.PostAsJsonAsync(endpoint,new UserActionRequest() { AccountId = currentSyncStatus.Id, Action = "resync" });
                if (httpResponse.IsSuccessStatusCode)
                {
                    var result = await httpResponse.Content.ReadAsStringAsync();
                }
                else
                {
                    var errorMessage = await httpResponse.Content.ReadAsStringAsync();
                    throw new APIException((int)httpResponse.StatusCode, $"API request failed: {errorMessage}");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }

        }

        private async Task<SyncStatusResponse> getLatestSyncStatus(string accessToken)
        {
            try
            {
                var basePath = _config["HumanAPI:DataAPI:apiBase"];

                using var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var endpoint = basePath + Constants.GET_ACCOUNT_ID;
                var httpResponse = await httpClient.GetAsync(endpoint);
                if (httpResponse.IsSuccessStatusCode)
                {
                    return  await httpResponse.Content.ReadFromJsonAsync<SyncStatusResponse>();
                }
                else
                {
                    var errorMessage = await httpResponse.Content.ReadAsStringAsync();
                    throw new APIException((int)httpResponse.StatusCode, $"API request failed: {errorMessage}");
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
