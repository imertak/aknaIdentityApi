using aknaIdentities_api.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aknaIdentity_api.Domain.Interfaces.Repositories
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task<Role> GetByNameAsync(string name);
        Task<List<Role>> GetRolesWithPermissionsAsync();
        Task<Role> GetRoleWithPermissionsAsync(int roleId);
        Task<List<Role>> GetUserRolesAsync(int userId);
        Task<bool> IsRoleNameExistsAsync(string name);
        Task<List<Role>> GetRolesByPermissionAsync(int permissionId);
    }
}
