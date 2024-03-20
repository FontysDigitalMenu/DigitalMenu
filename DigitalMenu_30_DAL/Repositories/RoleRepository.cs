using DigitalMenu_20_BLL.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;

namespace DigitalMenu_30_DAL.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly RoleManager<IdentityRole> _roleManager;

    private readonly UserManager<IdentityUser> _userManager;

    public RoleRepository(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public IEnumerable<IdentityRole> GetAll()
    {
        return _roleManager.Roles;
    }

    public async Task<IdentityUser?> AttachRoleToUser(string roleName, string userId)
    {
        IdentityUser? user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }

        return user;
    }

    public async Task<IdentityUser?> RevokeRoleFromUser(string roleName, string userId)
    {
        IdentityUser? user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            await _userManager.RemoveFromRoleAsync(user, roleName);
        }

        return user;
    }
}