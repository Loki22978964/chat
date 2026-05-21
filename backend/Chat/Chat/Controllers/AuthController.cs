using Chat.Models.DTO;
using Application.Auth.Service;
using Microsoft.AspNetCore.Mvc;

namespace Chat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(AccountService accountService): ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterUserRequest request)
        {
            await accountService.RegisterAsync(request.UserName, request.Email, request.Password);
            return NoContent();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginRequest request)
        {
            var token = await accountService.LoginAsync(request.Email, request.Password);
            return Ok(new { token });
        }
    }
}
