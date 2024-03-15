using DigitalMenu_10_Api.RequestModels;
using DigitalMenu_10_Api.ViewModels;
using DigitalMenu_20_BLL.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMenu_10_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;

        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService, SignInManager<IdentityUser> signInManager)
        {
            _roleService = roleService;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IEnumerable<RoleViewModel> Get()
        {
            return _roleService.GetAll().Select(x => new RoleViewModel
            {
                Name = x.Name
            });
        }

        [HttpPost("attachRoleToUser")]
        public async Task<IActionResult> AttachRoleToUser([FromBody] UserRoleRequest userRoleRequest)
        {
            IdentityUser? user = await _roleService.AttachRoleToUser(userRoleRequest.RoleName, userRoleRequest.UserId);
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            await _signInManager.RefreshSignInAsync(user);

            return Ok();
        }

        [HttpPost("revokeRoleFromUser")]
        public async Task RevokeRoleFromUser(UserRoleRequest userRoleRequest)
        {
            IdentityUser? user = await _roleService.RevokeRoleFromUser(userRoleRequest.RoleName, userRoleRequest.UserId);
            if (user != null)
            {
                await _signInManager.RefreshSignInAsync(user);
            }
        }
    }
}
