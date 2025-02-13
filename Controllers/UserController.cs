using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Entities;
using SignInApi.SetUnitOfWork;

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

        [HttpGet("get/{id}")]
        public async Task<ActionResult<UsersEntity>> Get(ulong id)
        {
            try
            {
                if (id == 0)
                    return BadRequest("Invalid user ID");

                UsersEntity? user = await _uof.UsersRepository.Get(id);

                if (user is null)
                    return NotFound("User not found");

                return Ok(user);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {e}");
            }
        }

        [HttpPost("create")]
        public async Task<ActionResult> Post([FromBody] UsersEntity user)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _uof.UsersRepository.Create(user);
                await _uof.Commit();

                return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {e}");
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> Delete(ulong id)
        {
            try
            {
                if (id == 0)
                    return BadRequest("Invalid user ID");

                var user = await _uof.UsersRepository.Delete(id);

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

        [HttpPut("update")]
        public async Task<ActionResult> Put([FromBody] UsersEntity user)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

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