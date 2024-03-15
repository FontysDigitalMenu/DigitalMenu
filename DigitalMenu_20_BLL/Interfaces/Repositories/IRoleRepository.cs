using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalMenu_20_BLL.Interfaces.Repositories
{
    public interface IRoleRepository
    {
        public IEnumerable<IdentityRole> GetAll();

        public Task<IdentityUser?> AttachRoleToUser(string roleName, string userId);

        public Task<IdentityUser?> RevokeRoleFromUser(string roleName, string userId);
    }
}
