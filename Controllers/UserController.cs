using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SignInApiEntities;
using Microsoft.AspNetCore.Identity;
using SignInApiAspCore.Controllers.DTOs;
using SignInApi.utils.responses;
using SignInApi.DTOs;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.RateLimiting;

namespace SignInApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("me", Name = "GetUser")]
        [EnableRateLimiting("readLimitPolicy")]
        public async Task<ActionResult> Me()
        {
            try
            {
                string? emailClaim = User.FindFirst(ClaimTypes.Email)?.Value?.Trim();

                if (string.IsNullOrWhiteSpace(emailClaim))
                    return Unauthorized(new ResponseBody<string>{
                        Body = null,
                        Message = "Token does not contain a valid email.",
                        Success = false,
                        Timestamp = DateTimeOffset.Now,
                        StatusCode = 401,
                    });

                ApplicationUser? user = await _userManager.FindByEmailAsync(emailClaim);

                if (user is null)
                    return NotFound(new ResponseBody<string>
                    {
                        Body = null,
                        Message = "User not found.",
                        Success = false,
                        Timestamp = DateTimeOffset.Now,
                        StatusCode = 404,
                    });

                return Ok(new ResponseBody<UserDTO>
                    {
                        Body = new UserDTO{ Id = user.Id, UserName = user.UserName,Email = user.Email,PhoneNumber = user.PhoneNumber},
                        Message = "User found.",
                        Success = true,
                        Timestamp = DateTimeOffset.Now,
                        StatusCode = 200,
                    }
                );

            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBody<string>
                    {
                        Body = e.Message,
                        Message = "Error the get user! Please try again later",
                        Success = false,
                        Timestamp = DateTimeOffset.Now,
                        StatusCode = StatusCodes.Status500InternalServerError,
                    });
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("delete", Name = "DeleteUser")]
        [EnableRateLimiting("deleteLimitPolicy")]
        public async Task<ActionResult> Delete()
        {
            try
            {
                string? email = User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrWhiteSpace(email))
                    return Unauthorized(new ResponseBody<string>{
                        Body = null,
                        Message = "Token does not contain a valid email.",
                        Success = false,
                        Timestamp = DateTimeOffset.Now,
                        StatusCode = 401,
                    });

                ApplicationUser? user = await _userManager.FindByEmailAsync(email);

                if (user is null)
                    return NotFound(new ResponseBody<string>
                    {
                        Body = null,
                        Message = "User not found",
                        Success = false,
                        Timestamp = DateTimeOffset.Now,
                        StatusCode = 404,
                    });

                IdentityResult result = await _userManager.DeleteAsync(user);

                if (!result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,new ResponseBody<IEnumerable<IdentityError>>
                    {
                        Body = result.Errors,
                        Message = "Delete failed",
                        Success = false,
                        Timestamp = DateTimeOffset.Now,
                        StatusCode = StatusCodes.Status500InternalServerError,
                    });
                }

                return Ok(new ResponseBody<string>
                    {
                        Body = null,
                        Message = "User deleted successfully.",
                        Success = true,
                        Timestamp = DateTimeOffset.Now,
                        StatusCode = 200,
                    }
                );
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBody<string>
                    {
                        Body = e.Message,
                        Message = "Error the delete user! Please try again later",
                        Success = false,
                        Timestamp = DateTimeOffset.Now,
                        StatusCode = StatusCodes.Status500InternalServerError,
                    });
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("update", Name = "UpdateUser")]
        [EnableRateLimiting("updateLimitPolicy")]
        public async Task<ActionResult> Update([FromBody] UpdateUserDto userDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ResponseBody<ValidationErrors> errorResponse = CreateErrorResponse(ModelState);
                    return BadRequest(errorResponse);
                }

                string? email = User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrWhiteSpace(email))
                    return Unauthorized(new ResponseBody<string>{
                        Body = null,
                        Message = "Token does not contain a valid email.",
                        Success = false,
                        Timestamp = DateTimeOffset.Now,
                        StatusCode = 401,
                    });

                ApplicationUser? user = await _userManager.FindByEmailAsync(email);

                if (user is null)
                    return NotFound(new ResponseBody<string>
                    {
                        Body = null,
                        Message = "User not found",
                        Success = false,
                        Timestamp = DateTimeOffset.Now,
                        StatusCode = 404,
                    });

                if (!string.IsNullOrWhiteSpace(userDto.Password))
                {
                    var passwordHasher = new PasswordHasher<ApplicationUser>();
                    user.PasswordHash = passwordHasher.HashPassword(user, userDto.Password);
                }

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,new ResponseBody<IEnumerable<IdentityError>>
                    {
                        Body = result.Errors,
                        Message = "Error the update user",
                        Success = false,
                        Timestamp = DateTimeOffset.Now,
                        StatusCode = StatusCodes.Status500InternalServerError,
                    });
                }

                return Ok(new ResponseBody<string>
                    {
                        Body = null,
                        Message = "User updated successfully.",
                        Success = true,
                        Timestamp = DateTimeOffset.Now,
                        StatusCode = 200,
                    }
                );
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBody<string>
                    {
                        Body = e.Message,
                        Message = "Error the update user! Please try again later",
                        Success = false,
                        Timestamp = DateTimeOffset.Now,
                        StatusCode = StatusCodes.Status500InternalServerError,
                    });
            }
        }

        private ResponseBody<ValidationErrors> CreateErrorResponse(ModelStateDictionary modelState)
        {
            ValidationErrors errorDict = new ValidationErrors();

            foreach (string key in modelState.Keys)
            {
                ModelStateEntry? state = modelState[key];
                if (state.Errors.Any())
                {
                    var errorMessages = state.Errors.Select(e => e.ErrorMessage).ToList();
                    errorDict.Errors.Add(key, errorMessages);
                }
            }

            return new ResponseBody<ValidationErrors>
            {
                Message = "Validation failed. Check the response body for errors.",
                Success = false,
                StatusCode = StatusCodes.Status400BadRequest, 
                Timestamp = DateTimeOffset.UtcNow,
                Body = errorDict, 
                Url = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.Path}"
            };
        }

    }
}
