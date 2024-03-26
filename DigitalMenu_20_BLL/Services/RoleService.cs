using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Interfaces.Services;
using Microsoft.AspNetCore.Identity;

namespace DigitalMenu_20_BLL.Services;

public class RoleService(IRoleRepository roleRepository) : IRoleService
{
    public IEnumerable<IdentityRole> GetAll()
    {
        return roleRepository.GetAll();
    }

    public async Task<IdentityUser?> AttachRoleToUser(string roleName, string userId)
    {
        return await roleRepository.AttachRoleToUser(roleName, userId);
    }

    public async Task<IdentityUser?> RevokeRoleFromUser(string roleName, string userId)
    {
        return await roleRepository.RevokeRoleFromUser(roleName, userId);
    }
}