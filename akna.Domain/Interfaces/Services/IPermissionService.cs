using aknaIdentities_api.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace aknaIdentity_api.Domain.Interfaces.Services
{
    public interface IPermissionService
    {
        Task<Permission> CreatePermissionAsync(string name, string description = null);
        Task<Permission> GetByIdAsync(int id);
        Task<Permission> GetByNameAsync(string name);
        Task<List<Permission>> GetAllPermissionsAsync();
        Task<List<Permission>> GetRolePermissionsAsync(int roleId);
        Task<List<Permission>> GetUserPermissionsAsync(int userId);
        Task<bool> UserHasPermissionAsync(int userId, string permissionName);
        Task<bool> UserHasAnyPermissionAsync(int userId, List<string> permissionNames);
        Task<bool> UserHasAllPermissionsAsync(int userId, List<string> permissionNames);
        Task<Permission> UpdatePermissionAsync(int id, string name, string description = null);
        Task DeletePermissionAsync(int id, int deletedByUserId);
        Task<List<Permission>> GetPermissionsByNamesAsync(List<string> permissionNames);
    }
}