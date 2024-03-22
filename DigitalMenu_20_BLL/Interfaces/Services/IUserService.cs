using Microsoft.AspNetCore.Identity;

namespace DigitalMenu_20_BLL.Interfaces.Services;

public interface IUserService
{
    public List<IdentityUser> SearchByEmail(string email);
}