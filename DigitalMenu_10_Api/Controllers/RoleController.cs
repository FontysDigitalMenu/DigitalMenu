using DigitalMenu_10_Api.RequestModels;
using DigitalMenu_10_Api.ViewModels;
using DigitalMenu_20_BLL.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMenu_10_Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class RoleController(IRoleService roleService, SignInManager<IdentityUser> signInManager) : Controller
{
    [HttpGet]
    public IEnumerable<RoleViewModel> Get()
    {
        return roleService.GetAll().Select(x => new RoleViewModel
        {
            Name = x.Name,
        });
    }

    [HttpPost("attachRoleToUser")]
    public async Task<IActionResult> AttachRoleToUser([FromBody] UserRoleRequest userRoleRequest)
    {
        IdentityUser? user = await roleService.AttachRoleToUser(userRoleRequest.RoleName, userRoleRequest.UserId);
        if (user == null)
        {
            return NotFound(new { Message = "User not found" });
        }

        await signInManager.RefreshSignInAsync(user);

        return Ok();
    }

    [HttpPost("revokeRoleFromUser")]
    public async Task RevokeRoleFromUser(UserRoleRequest userRoleRequest)
    {
        IdentityUser? user = await roleService.RevokeRoleFromUser(userRoleRequest.RoleName, userRoleRequest.UserId);
        if (user != null)
        {
            await signInManager.RefreshSignInAsync(user);
        }
    }
}