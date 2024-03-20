using Microsoft.AspNetCore.Identity;

namespace DigitalMenu_20_BLL.Interfaces.Services;

public interface IRoleService
{
    public IEnumerable<IdentityRole> GetAll();

    public Task<IdentityUser?> AttachRoleToUser(string roleName, string userId);

    public Task<IdentityUser?> RevokeRoleFromUser(string roleName, string userId);
}