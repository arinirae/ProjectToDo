using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectToDo.Services;
using ProjectToDo.Services.Dto;

namespace ProjectToDo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _iAuthService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthenticationService iAuthService, ILogger<AuthController> logger)
        {
            _iAuthService = iAuthService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto input)
        {
            try
            {
                var result = await _iAuthService.Login(input.Username, input.Password);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                _logger.LogError(ex.Message);

                var obj = new
                {
                    msg = "Login Gagal!"
                };

                return StatusCode(500, obj);
            }
        }
    }
}
