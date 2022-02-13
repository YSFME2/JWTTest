using JWTTest.Auth;
using JWTTest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWTTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegistrationAsync([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authService.RegisterAsync(model);
            if (!result.IsAuthenticated)
                return BadRequest(result.Message);
            
            return Ok(result);
        }
        [HttpPost("token")]
        public async Task<IActionResult> GetTokenAsync([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.GetTokenAsync(model);
            if (!result.IsAuthenticated)
                return BadRequest(result.Message);
            
            return Ok(result);
        }
        [HttpPost("addRole")]
        public async Task<IActionResult> AssignRoleAsync([FromBody] UserRole model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.AddRoleToUser(model);
            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);
            
            return Ok("Done!");
        }

    }
}
