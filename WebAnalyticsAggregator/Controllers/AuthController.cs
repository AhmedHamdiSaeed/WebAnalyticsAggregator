using Application.Interfaces;
using DTOs.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAnalyticsAggregator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private ILogger<AuthController> _logger;

        public AuthController(IUserService userService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest request)
        {
            var result = await _userService.RegisterAsync(request.Name,request.Email,request.Password);
            if(result.IsSuccess)
             return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            var result = await _userService.LoginAsync(request.Email, request.Password);
            if (result.IsSuccess)
                return Ok(result);
            else if (result.Code== "INVALID_CREDENTIALS")
                return Unauthorized(result);
            else
                return BadRequest(result);  
        }
    }
}
