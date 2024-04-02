using DigitalMenu_20_BLL.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;

namespace DigitalMenu_30_DAL.Repositories;

public class UserRepository(UserManager<IdentityUser> userManager) : IUserRepository
{
    public List<IdentityUser> SearchByEmail(string email)
    {
        return userManager.Users.Where(u => u.Email != null && u.Email.Contains(email)).ToList();
    }
}