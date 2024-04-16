using DigitalMenu_20_BLL.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;

namespace DigitalMenu_30_DAL.Repositories;

public class RoleRepository(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    : IRoleRepository
{
    public IEnumerable<IdentityRole> GetAll()
    {
        return roleManager.Roles;
    }
    
    public async Task<IdentityUser?> AttachRoleToUser(string roleName, string userId)
    {
        IdentityUser? user = await userManager.FindByIdAsync(userId);
        if (user != null)
        {
            await userManager.AddToRoleAsync(user, roleName);
        }
        
        return user;
    }
    
    public async Task<IdentityUser?> RevokeRoleFromUser(string roleName, string userId)
    {
        IdentityUser? user = await userManager.FindByIdAsync(userId);
        if (user != null)
        {
            await userManager.RemoveFromRoleAsync(user, roleName);
        }
        
        return user;
    }
}