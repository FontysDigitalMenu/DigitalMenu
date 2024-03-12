using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalMenu_20_BLL.Interfaces.Repositories
{
    public interface IUserRepository
    {
        public List<IdentityUser> SearchByEmail(string email);
    }
}
