using aknaIdentities_api.Domain.Entities;
using aknaIdentity_api.Domain.Interfaces.Contracts;
using aknaIdentity_api.Domain.Interfaces.Repositories;
using aknaIdentity_api.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aknaIdentity_api.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(IRoleRepository roleRepository, IPermissionRepository permissionRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Role> CreateRoleAsync(string name, string description = null)
        {
            if (await _roleRepository.IsRoleNameExistsAsync(name))
                throw new InvalidOperationException($"Role '{name}' already exists.");

            var role = new Role
            {
                Name = name,
                Description = description
            };

            await _roleRepository.AddAsync(role);
            await _unitOfWork.SaveChangesAsync();

            return role;
        }

        public async Task<Role> GetByIdAsync(int id)
        {
            return await _roleRepository.GetByIdAsync(id);
        }

        public async Task<Role> GetByNameAsync(string name)
        {
            return await _roleRepository.GetByNameAsync(name);
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await _roleRepository.GetAllAsync();
        }

        public async Task<List<Role>> GetRolesWithPermissionsAsync()
        {
            return await _roleRepository.GetRolesWithPermissionsAsync();
        }

        public async Task<Role> GetRoleWithPermissionsAsync(int roleId)
        {
            return await _roleRepository.GetRoleWithPermissionsAsync(roleId);
        }

        public async Task<List<Role>> GetUserRolesAsync(int userId)
        {
            return await _roleRepository.GetUserRolesAsync(userId);
        }

        public async Task<Role> UpdateRoleAsync(int id, string name, string description = null)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
                throw new ArgumentException("Role not found.");

            if (role.Name != name && await _roleRepository.IsRoleNameExistsAsync(name))
                throw new InvalidOperationException($"Role '{name}' already exists.");

            role.Name = name;
            role.Description = description;

            await _roleRepository.UpdateAsync(role);
            await _unitOfWork.SaveChangesAsync();

            return role;
        }

        public async Task DeleteRoleAsync(int id, int deletedByUserId)
        {
            await _roleRepository.SoftDeleteAsync(id, deletedByUserId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AssignPermissionsToRoleAsync(int roleId, List<int> permissionIds)
        {
            var role = await _roleRepository.GetRoleWithPermissionsAsync(roleId);
            if (role == null)
                throw new ArgumentException("Role not found.");

            var permissions = await _permissionRepository.GetByIdsAsync(permissionIds);

            // Remove existing permissions not in the new list
            var permissionsToRemove = role.Permissions.Where(p => !permissionIds.Contains(p.Id)).ToList();
            foreach (var permission in permissionsToRemove)
            {
                role.Permissions.Remove(permission);
            }

            // Add new permissions
            var existingPermissionIds = role.Permissions.Select(p => p.Id).ToList();
            var permissionsToAdd = permissions.Where(p => !existingPermissionIds.Contains(p.Id)).ToList();
            foreach (var permission in permissionsToAdd)
            {
                role.Permissions.Add(permission);
            }

            await _roleRepository.UpdateAsync(role);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemovePermissionsFromRoleAsync(int roleId, List<int> permissionIds)
        {
            var role = await _roleRepository.GetRoleWithPermissionsAsync(roleId);
            if (role == null)
                throw new ArgumentException("Role not found.");

            var permissionsToRemove = role.Permissions.Where(p => permissionIds.Contains(p.Id)).ToList();
            foreach (var permission in permissionsToRemove)
            {
                role.Permissions.Remove(permission);
            }

            await _roleRepository.UpdateAsync(role);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AssignRoleToUserAsync(int userId, int roleId)
        {
            var user = await _userRepository.GetUserWithRolesAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found.");

            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
                throw new ArgumentException("Role not found.");

            if (!user.Roles.Any(r => r.Id == roleId))
            {
                user.Roles.Add(role);
                await _userRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task RemoveRoleFromUserAsync(int userId, int roleId)
        {
            var user = await _userRepository.GetUserWithRolesAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found.");

            var roleToRemove = user.Roles.FirstOrDefault(r => r.Id == roleId);
            if (roleToRemove != null)
            {
                user.Roles.Remove(roleToRemove);
                await _userRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task AssignRolesToUserAsync(int userId, List<int> roleIds)
        {
            var user = await _userRepository.GetUserWithRolesAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found.");

            var roles = await _roleRepository.GetByIdsAsync(roleIds);

            // Remove existing roles not in the new list
            var rolesToRemove = user.Roles.Where(r => !roleIds.Contains(r.Id)).ToList();
            foreach (var role in rolesToRemove)
            {
                user.Roles.Remove(role);
            }

            // Add new roles
            var existingRoleIds = user.Roles.Select(r => r.Id).ToList();
            var rolesToAdd = roles.Where(r => !existingRoleIds.Contains(r.Id)).ToList();
            foreach (var role in rolesToAdd)
            {
                user.Roles.Add(role);
            }

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<Role>> GetRolesByPermissionAsync(int permissionId)
        {
            return await _roleRepository.GetRolesByPermissionAsync(permissionId);
        }
    }
}