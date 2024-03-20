using Microsoft.AspNetCore.Identity;

namespace DigitalMenu_20_BLL.Interfaces.Repositories;

public interface IUserRepository
{
    public List<IdentityUser> SearchByEmail(string email);
}