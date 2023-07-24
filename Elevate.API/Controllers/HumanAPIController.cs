using Elevate.Interface.HumanAPI;
using Elevate.Model.HumanAPI;
using Elevate.Utilities.Errors;
using Elevate.Utilities.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Elevate.API.Controllers
{
    public class HumanAPIController : BaseAPIController
    {
        private readonly IHumanAPIService _humanAPIService;
        private readonly ILogger<HumanAPIController> _logger;
        public HumanAPIController(IHumanAPIService humanAPIService, ILogger<HumanAPIController> logger)
        {
             _humanAPIService = humanAPIService;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<string>> GetSessionToken(string email)
        {
            try
            {
                return await _humanAPIService.RequestSessionToken(email);
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
        public async Task<ActionResult<string>> RequestPublicToken(string email)
        {
            try
            {
                return await _humanAPIService.RequestPublicToken(email);
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

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<string>> GetUserAccessToken(SessionTokenObject accessTokenRequest)
        {
            try
            {
                return await _humanAPIService.GetUserAccessToken(accessTokenRequest);
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
