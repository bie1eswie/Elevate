﻿using Elevate.Interface.HumanAPI;
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

        private readonly string _dataAPIBase;

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
            _dataAPIBase = _config["HumanAPI:DataAPI:apiBase"];

            if (string.IsNullOrEmpty(_dataAPIBase))
            {
                throw new ApplicationException("HumanAPI configuration is incomplete.");
            }
        }

        public async Task<IReadOnlyList<ActivitySummary>> GetActivitySummary(string email)
        {
            try
            {
                var clientUserId = (await _userManagerService.FindByEmailAsync(email))?.Id;
                if (string.IsNullOrEmpty(clientUserId))
                {
                    throw new InvalidOperationException("User not found.");
                }

                var accessToken = (await _humanAPIRepository.GetHumanAPIUser(clientUserId))?.AccessToken;
                if (string.IsNullOrEmpty(accessToken))
                {
                    throw new InvalidOperationException("Access token not found for the user.");
                }

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
                if (string.IsNullOrEmpty(vitalName))
                {
                    throw new ArgumentException("Invalid vital name.");
                }

                using var httpClient = CreateAuthorizedHttpClient(accessToken);

                if (!Constants.VitalsDataEndPoints.TryGetValue(vitalName, out var endpoint))
                {
                    throw new ArgumentException("Invalid vital name.");
                }

                var fullEndpoint = _dataAPIBase + endpoint;
                var httpResponse = await httpClient.GetAsync(fullEndpoint);

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
    }
}
