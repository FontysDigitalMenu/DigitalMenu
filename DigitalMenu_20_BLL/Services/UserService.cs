using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Interfaces.Services;
using Microsoft.AspNetCore.Identity;

namespace DigitalMenu_20_BLL.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    public List<IdentityUser> SearchByEmail(string email)
    {
        return userRepository.SearchByEmail(email);
    }
}