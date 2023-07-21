using Elevate.DTO.DTOs;
using Elevate.Interface.HumanAPI;
using Elevate.Interface.Identity;
using Elevate.Model.Identity;
using Elevate.Utilities.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Elevate.API.Controllers
{
    public class AccountController : BaseAPIController
    {
        private readonly IUserManagerService _userManagerService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AccountController> _logger;
        private readonly IHumanAPIService _humanAPIService;
        public AccountController(IUserManagerService userManagerService, ITokenService tokenService, ILogger<AccountController> logger, IHumanAPIService humanAPIService)
        {
            _tokenService = tokenService;
            _userManagerService = userManagerService;
            _logger = logger;
            _humanAPIService = humanAPIService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            try
            {
                var user = await _userManagerService.FindByEmailFromClaimsPrinciple(this.HttpContext.User);
                return new UserDto
                {
                    Email = user.Email,
                    Token = _tokenService.CreateToken(user),
                    DisplayName = user.DisplayName
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return new BadRequestObjectResult(new APIException(500, "An error occurred while processing the request."));
            }
        }
        [HttpGet("emailexists")]
        public async Task<ActionResult<bool>> CheckEmailExistsAsync([FromQuery] string email)
        {
            return await _userManagerService.CheckEmailExistsAsync(email);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDTO loginDto)
        {
            try
            {
                var user = await _userManagerService.FindByEmailAsync(loginDto.Email);

                if (user == null)
                {
                    return Unauthorized(new APIException(401, "Unauthorized"));
                }

                var result = await _userManagerService.CheckPasswordSignIn(user, loginDto.Password);

                if (!result.Succeeded)
                {
                    //We don't want to give any hint whether password is correct
                    return Unauthorized(new APIException(401, "Unauthorized"));
                }

                return new UserDto
                {
                    Email = user.Email,
                    Token = _tokenService.CreateToken(user),
                    DisplayName = user.DisplayName,
                    PublicToken = await _humanAPIService.RequestPublicToken(user.Email)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return new BadRequestObjectResult(new APIException(500, "An error occurred while processing the request."));
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDTO registerDto)
        {
            try
            {
                if (CheckEmailExistsAsync(registerDto.Email).Result.Value)
                {
                    return new BadRequestObjectResult(new APIValidationError { Errors = new[] { "Email address is already in use" } });
                }
                var user = new AppUser
                {
                    DisplayName = string.Empty,
                    Email = registerDto.Email,
                    UserName = registerDto.Email
                };
                var result = await _userManagerService.CreateUser(user, registerDto.Password);
                if (!result.Succeeded) return BadRequest(new APIException(400));
                return new UserDto
                {
                    DisplayName = string.Empty,
                    Token = _tokenService.CreateToken(user),
                    Email = registerDto.Email
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return new BadRequestObjectResult(new APIException(500));
            }
        }
    }
}
