using Microsoft.AspNetCore.Mvc;
using Learning_Backend.Contracts;
using Learning_Backend.DTOS;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Learning_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IRepoWrapper _repoWrapper;
        private readonly RoleService _roleService;

        public UserController(IRepoWrapper repoWrapper, RoleService roleService)
        {
            _repoWrapper = repoWrapper;
            _roleService = roleService;
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
            var roleClaim = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            if (roleClaim != null && int.TryParse(roleClaim, out int role))
            {
                if (role != 1) 
                {
                    return Forbid();
                }
            }
            else
            {
                return Unauthorized();
            }

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

        [HttpGet("roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roleClaim = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            if (roleClaim != null && int.TryParse(roleClaim, out int role))
            {
                if (role != 1)
                {
                    return Forbid();
                }
            }
            else
            {
                return Unauthorized();
            }

            var res = await _roleService.GetRolesAsync();

            if (res != null)
            {
                return Ok(res);
            }
            else
            {
                return NotFound(res);
            }
        }

        [Authorize]
        [HttpPost("updateUser")]
        public async Task<IActionResult> UpdateUser([FromForm] UpdateUserDTO model)
        {
            var userIdClaim = HttpContext.User.FindFirst("UserId")?.Value;

            var res = await _repoWrapper.User.UpdateUsers(model, int.Parse(userIdClaim));

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
        public async Task<IActionResult> CreateUser([FromForm] RegisterUserDTO user)
        {
                var res = await _repoWrapper.User.RegisterUser(user);

                if (res.StatusCode == 200)
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

            if (res.StatusCode == 201)
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
