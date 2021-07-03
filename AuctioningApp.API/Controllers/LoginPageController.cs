using AuctioningApp.Domain.BLL.ServiceInterfaces;
using AuctioningApp.Domain.Models.DTO;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctioningApp.API.Controllers
{
    [Route("auth")]
    [EnableCors("CorsPolicy")]
    public class LoginPageController : ControllerBase
    {
        private readonly ILoginService loginService;

        public LoginPageController(ILoginService service)
        {
            loginService = service;
        }

        // /auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUser register)
        {
            try
            {
                await loginService.Register(register);
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

        // /auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginInfo user)
        {
            try
            {
                var tokenString = await loginService.Login(user);
                return Ok(new { tokenString });
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}
