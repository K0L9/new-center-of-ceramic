using CenterOfCeramic.Data.Helpers;
using CenterOfCeramic.Interfaces;
using CenterOfCeramic.Models.Identity;
using CenterOfCeramic.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CenterOfCeramic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AccountService _accountService;

        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }


        [HttpPost("register-user")]
        public async Task<IActionResult> RegisterUser(RegisterUserDTO registerUser)
        {
            try
            {
                var response = await _accountService.RegisterUserAsync(registerUser);
                if (response.Succeeded)
                    return Ok();
                return BadRequest(response);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpPost("login-user")]
        public async Task<IActionResult> LoginUser(LoginUserDTO loginUser)
        {
            try
            {
                var response = await _accountService.LoginUserAsync(loginUser);
                if (response.Succeeded)
                    return Ok(((IResult<TokenResponse>)response).Data);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
