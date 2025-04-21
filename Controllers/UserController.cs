using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Entities;
using SignInApi.SetUnitOfWork;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _uof;

        public UserController(IUnitOfWork uof)
        {
            _uof = uof;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("me")]
        public async Task<ActionResult<UsersEntity>> Me()
        {
            try
            {
                string? emailClaim = User.FindFirst(ClaimTypes.Email)?.Value.Trim();

                

                if (string.IsNullOrWhiteSpace(emailClaim))
                    return Unauthorized("Token does not contain a valid email.");

                Console.WriteLine($"Email: {emailClaim}");

                UsersEntity? user = await _uof.UsersRepository.Get(emailClaim);

                Console.WriteLine($"User: {user}");

                if (user is null)
                    return NotFound("User not found.");

                return Ok(user);
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
                string? email = User.FindFirst(ClaimTypes.Sid)?.Value;

                if (string.IsNullOrWhiteSpace(email))
                    return BadRequest("Invalid user ID in token");

                UsersEntity? user = await _uof.UsersRepository.Delete(email);

                if (user is null)
                    return NotFound("User not found");

                await _uof.Commit();
                return NoContent();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {e.Message}");
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("update")]
        public async Task<ActionResult> Update([FromBody] UsersEntity user)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                string? email = User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrWhiteSpace(email))
                    return BadRequest("Invalid user ID in token");

                user.Email = email;

                var userUpdated = await _uof.UsersRepository.Update(user);

                if (userUpdated is null)
                    return NotFound("User not found");

                await _uof.Commit();
                return Ok($"User updated! {userUpdated}");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {e.Message}");
            }
        }

    }
}