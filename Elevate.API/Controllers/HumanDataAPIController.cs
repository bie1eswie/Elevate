using Elevate.Interface.HumanAPI;
using Elevate.Utilities.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Elevate.API.Controllers
{
    public class HumanDataAPIController : ControllerBase
    {
        private readonly IHumanDataAPIService _humanDataAPIService;
        private readonly ILogger<HumanAPIController> _logger;
        public HumanDataAPIController(IHumanDataAPIService humanDataAPIService, ILogger<HumanAPIController> logger)
        {
            _humanDataAPIService = humanDataAPIService;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetVitalsData(string accessToken, string vitalName)
        {
            try
            {
                return Ok(await _humanDataAPIService.GetVitals(accessToken, vitalName));
            }
            catch (APIException ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return new BadRequestObjectResult(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return new BadRequestObjectResult(new APIException(500, "An error occurred while processing the request."));
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetActivitySummary(string accessToken)
        {
            try
            {
                return Ok(await _humanDataAPIService.GetActivitySummary(accessToken));
            }
            catch (APIException ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return new BadRequestObjectResult(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return new BadRequestObjectResult(new APIException(500, "An error occurred while processing the request."));
            }
        }
    }
}
