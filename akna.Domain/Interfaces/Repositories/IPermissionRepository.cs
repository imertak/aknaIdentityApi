using aknaIdentities_api.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aknaIdentity_api.Domain.Interfaces.Repositories
{
    public interface IPermissionRepository : IRepository<Permission>
    {
        Task<Permission> GetByNameAsync(string name);
        Task<List<Permission>> GetRolePermissionsAsync(int roleId);
        Task<List<Permission>> GetUserPermissionsAsync(int userId);
        Task<bool> IsPermissionNameExistsAsync(string name);
        Task<bool> UserHasPermissionAsync(int userId, string permissionName);
        Task<List<Permission>> GetPermissionsByNamesAsync(List<string> permissionNames);
    }
}
