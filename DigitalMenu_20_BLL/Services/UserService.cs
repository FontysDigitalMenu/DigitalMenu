using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Interfaces.Services;
using Microsoft.AspNetCore.Identity;

namespace DigitalMenu_20_BLL.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public List<IdentityUser> SearchByEmail(string email)
    {
        return _userRepository.SearchByEmail(email);
    }
}