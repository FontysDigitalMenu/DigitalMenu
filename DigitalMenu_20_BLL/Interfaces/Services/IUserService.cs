using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalMenu_20_BLL.Interfaces.Services
{
    public interface IUserService
    {
        public List<IdentityUser> SearchByEmail(string email);
    }
}
