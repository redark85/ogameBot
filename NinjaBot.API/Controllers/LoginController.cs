using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using NinjaBot.Core.Services;
using NinjaBot.Domain.Dtos;
using NinjaBot.Domain.Models;
using NinjaBot.Shared.Dtos.Requests;

namespace NinjaBot.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController: ControllerBase
    {
        private readonly OGameLoginService _loginService;

        public LoginController(OGameLoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] LoginRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Email y password son requeridos.");

            var result = await _loginService.LoginWithCredentialsAsync(request);
            return Ok(result);
        }
    }
}
