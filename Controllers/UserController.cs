using Microsoft.AspNetCore.Mvc;
using Learning_Backend.Contracts;
using Learning_Backend.DTOS;
using Learning_Backend.Models.LearningDatabaseModels;
using Microsoft.AspNetCore.Authorization;

namespace Learning_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IRepoWrapper _repoWrapper;

        public UserController(IRepoWrapper repoWrapper)
        {
            _repoWrapper = repoWrapper;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
                var res = await _repoWrapper.User.GetUserById(id);

                if (res != null && res.StatusCode == 200)
                {
                    return Ok(res);
                }
                else
                {
                    return NotFound(res);
                }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
           
                var res = await _repoWrapper.User.GetAllUsers();

                if (res != null && res.StatusCode == 200)
                {
                    return Ok(res);
                }
                else
                {
                    return NotFound(res);
                }
        }

        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<IActionResult> CreateUser([FromBody] RegisterUserDTO user)
        {
                var res = await _repoWrapper.User.RegisterUser(user);

                if (res != null && res.StatusCode == 201)
                {
                    return Ok(res);
                }
                else
                {
                    return NotFound(res);
                }
        }

        [AllowAnonymous]
        [HttpPost("signin")]
        public async Task<IActionResult> Login(LoginUserDTO model)
        {
            var res = await _repoWrapper.User.LoginRequest(model);

            if (res != null && res.StatusCode == 201)
            {
                return Ok(res);
            }
            else
            {
                return NotFound(res);
            }
        }
    }
}
