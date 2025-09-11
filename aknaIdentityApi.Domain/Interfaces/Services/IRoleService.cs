// IRoleService.cs
using aknaIdentities_api.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace aknaIdentity_api.Domain.Interfaces.Services
{
    public interface IRoleService
    {
        Task<Role> CreateRoleAsync(string name, string description = null);
        Task<Role> GetByIdAsync(int id);
        Task<Role> GetByNameAsync(string name);
        Task<List<Role>> GetAllRolesAsync();
        Task<List<Role>> GetRolesWithPermissionsAsync();
        Task<Role> GetRoleWithPermissionsAsync(int roleId);
        Task<List<Role>> GetUserRolesAsync(int userId);
        Task<Role> UpdateRoleAsync(int id, string name, string description = null);
        Task DeleteRoleAsync(int id, int deletedByUserId);
        Task AssignPermissionsToRoleAsync(int roleId, List<int> permissionIds);
        Task RemovePermissionsFromRoleAsync(int roleId, List<int> permissionIds);
        Task AssignRoleToUserAsync(int userId, int roleId);
        Task RemoveRoleFromUserAsync(int userId, int roleId);
        Task AssignRolesToUserAsync(int userId, List<int> roleIds);
        Task<List<Role>> GetRolesByPermissionAsync(int permissionId);
    }
}