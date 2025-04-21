using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Entities;
using SignInApi.Services.IServices;
using SignInApiEntities;

namespace SignInApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthController(
            ITokenService tokenService,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            string email = model.Email.Trim();
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(model.Password))
                return BadRequest("Email and Password must be provided.");

            var user = await _userManager.FindByEmailAsync(email);

            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName!),
                    new Claim(ClaimTypes.Email, user.Email.Trim()!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = _tokenService.GenerateAccessToken(authClaims, _configuration);
                var refreshToken = _tokenService.GenerateRefreshToken();

                _ = int.TryParse(_configuration["jwt:RefreshTokenValidityInMinutes"], out int refreshTokenValidity);

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(refreshTokenValidity);
                await _userManager.UpdateAsync(user);

                return Ok(new
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    RefreshToken = refreshToken,
                    Expiration = token.ValidTo
                });
            }

            return Unauthorized("Invalid email or password.");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto model)
        {
            try 
            {
                string email = model.Email.Trim();

                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(model.Password))
                    return BadRequest("Email and Password must be provided.");

                var userExists = await _userManager.FindByEmailAsync(email);

                if (userExists != null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest,
                        new Response { Status = "Error", Message = "User already exists." });
                }

                var user = new ApplicationUser
                {
                    Email = email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = model.Name
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        Status = "Error",
                        Message = "User creation failed.",
                        Errors = errors
                    });
                }

                return Ok(new Response { Status = "Success", Message = "User created successfully." });
            }
            catch (Exception e)
            {
                throw new Exception($"Error: \n{e}");
            }
            
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenModel tokenModel)
        {
            if (tokenModel is null || string.IsNullOrWhiteSpace(tokenModel.AcessToken) || string.IsNullOrWhiteSpace(tokenModel.RefreshToken))
                return BadRequest("Invalid client request");

            var principal = _tokenService.GetPrincipalFromExpiredToken(tokenModel.AcessToken, _configuration);

            if (principal == null)
                return BadRequest("Invalid access token or refresh token");

            var username = principal.Identity?.Name;

            if (string.IsNullOrWhiteSpace(username))
                return BadRequest("Invalid token data");

            var user = await _userManager.FindByNameAsync(username);

            if (user == null || user.RefreshToken != tokenModel.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return BadRequest("Invalid access token or refresh token");

            var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims.ToList(), _configuration);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                RefreshToken = newRefreshToken
            });
        }

        [Authorize]
        [HttpPost]
        [Route("revoke/{email}")]
        public async Task<IActionResult> Revoke(string email)
        {
            var user = await this._userManager.FindByEmailAsync(email);

            if (user == null) return BadRequest("Invalid email");

            user.RefreshToken = null;

            await _userManager.UpdateAsync(user);

            return NoContent();

        }

    }
}