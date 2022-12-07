using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TokensNewUser.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        public static List<User> users = new List<User>
        {
            new User{ Id = "1", Name = "Ramis",Email = "ramis123@gmail.com", Create = DateTime.Now, Password = "123"},
            new User{Id = "2", Name = "Rashid", Email = "rashid123@gmail.com", Create = DateTime.Now, Password = "456"}
        };
        public IConfiguration _conf;
        public LoginController(IConfiguration conf)
        {
            _conf = conf;
        }

        [HttpPost("{login}")]
        public async Task<IActionResult> Login(LoginRequest login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var res = await GetUser(login.passqord, login.email);
            if (res is not null)
            {
                var claim = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, _conf["token:subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim("Id", res.Id),
                    new Claim("Name", res.Name),
                    new Claim("Email",res.Email),
                };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_conf["token:key"]));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    _conf["token:issuer"],
                    _conf["token:audience"],
                    claim,
                    expires: DateTime.UtcNow.AddMinutes(10),
                    signingCredentials: signIn);
                var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
                var authResponse = new AuthResponse { AccessToken = tokenStr };

                return Ok(authResponse);
            }
            else
            {
                return BadRequest("Invalid credentials");
            }
        }

        private async Task<User> GetUser(string passqord, string email)
        {
            return await Task.FromResult(users.FirstOrDefault(a => a.Email == email || a.Password == passqord));
        }
    }
}
