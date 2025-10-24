using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Test_PoojaDoshi.Controllers
{

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration config, ILogger<AuthController> logger)
        {
            _config = config;
            _logger = logger;
        }

        /// <summary>
        /// Get a JWT for testing. In real apps, validate user credentials from a store.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("token")]
        public IActionResult Token([FromBody] LoginRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest("Username and password are required.");

            // For Demo Only: accept any username = admin and password == 'password'
            if (string.Equals(req.Username, "admin", StringComparison.Ordinal) && string.Equals(req.Password, "password", StringComparison.Ordinal))
            {
                var jwtSection = _config.GetSection("Jwt");
                var issuer = jwtSection["Issuer"]!;
                var audience = jwtSection["Audience"]!;
                var key = jwtSection["Key"]!;

                var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, req.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, req.Username),
            new Claim(ClaimTypes.Role, "User")
        };

                var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(1), // expires in 1 hour
                    signingCredentials: creds);

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new { access_token = tokenString, token_type = "Bearer", expires_in = 7200 });
            }
            return Unauthorized();


        }

        public class LoginRequest
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
    }
}
