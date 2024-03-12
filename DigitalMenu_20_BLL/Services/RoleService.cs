using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalMenu_20_BLL.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public IEnumerable<IdentityRole> GetAll()
        {
            return _roleRepository.GetAll();
        }

        public async Task<IdentityUser?> AttachRoleToUser(string roleName, string userId)
        {
            return await _roleRepository.AttachRoleToUser(roleName, userId);
        }

        public async Task<IdentityUser?> RevokeRoleFromUser(string roleName, string userId)
        {
            return await _roleRepository.RevokeRoleFromUser(roleName, userId);
        }
    }
}
