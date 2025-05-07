using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SignInApiEntities;
using Microsoft.AspNetCore.Identity;
using SignInApiAspCore.Controllers.DTOs;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("me")]
        public async Task<ActionResult> Me()
        {
            try
            {
                string? emailClaim = User.FindFirst(ClaimTypes.Email)?.Value?.Trim();

                if (string.IsNullOrWhiteSpace(emailClaim))
                    return Unauthorized("Token does not contain a valid email.");

                ApplicationUser? user = await _userManager.FindByEmailAsync(emailClaim);

                if (user is null)
                    return NotFound("User not found.");

                return Ok(new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.PhoneNumber
                });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {e.Message}");
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("delete")]
        public async Task<ActionResult> Delete()
        {
            try
            {
                string? email = User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrWhiteSpace(email))
                    return BadRequest("Invalid email in token");

                ApplicationUser? user = await _userManager.FindByEmailAsync(email);

                if (user is null)
                    return NotFound("User not found");

                var result = await _userManager.DeleteAsync(user);

                if (!result.Succeeded)
                    return StatusCode(StatusCodes.Status500InternalServerError, "Delete failed");

                return Ok("User deleted successfully.");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {e.Message}");
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("update")]
        public async Task<ActionResult> Update([FromBody] UpdateUserDto userDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                string? email = User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrWhiteSpace(email))
                    return BadRequest("Invalid email in token");

                ApplicationUser? user = await _userManager.FindByEmailAsync(email);

                if (user is null)
                    return NotFound("User not found.");

                if (!string.IsNullOrWhiteSpace(userDto.Password))
                {
                    var passwordHasher = new PasswordHasher<ApplicationUser>();
                    user.PasswordHash = passwordHasher.HashPassword(user, userDto.Password);
                }

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    Console.WriteLine(result);
                    return StatusCode(StatusCodes.Status500InternalServerError, "Update failed");
                }

                return Ok("User updated successfully.");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {e.Message}");
            }
        }


    }
}
