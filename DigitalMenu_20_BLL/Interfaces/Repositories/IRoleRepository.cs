using Microsoft.AspNetCore.Identity;

namespace DigitalMenu_20_BLL.Interfaces.Repositories;

public interface IRoleRepository
{
    public IEnumerable<IdentityRole> GetAll();

    public Task<IdentityUser?> AttachRoleToUser(string roleName, string userId);

    public Task<IdentityUser?> RevokeRoleFromUser(string roleName, string userId);
}