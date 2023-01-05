using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace MyTest.Controllers
{
    // [Authorize]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {

        public AuthorizeController()
        {
        }

        [HttpPost("/login")]
        public async Task<IActionResult> Login([FromBody] UserInfo user)
        {
            if(ValidateLogin(user))
            {
                var claims = new List<Claim>
                {
                    new Claim("user", user.UserId!),
                    new Claim("role", "Member")
                };

                await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", "user", "role")));

                return Ok("ok");
            }
            return Unauthorized();
        }

        private bool ValidateLogin(UserInfo user)
        {
            // For this sample, all logins are successful.
            return true;
        }

        public class UserInfo {
            public string? UserId { get; set; }
            public string? Password { get; set; }
        }
    }
}
