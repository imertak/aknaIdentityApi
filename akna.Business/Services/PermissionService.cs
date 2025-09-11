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
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PermissionService(IPermissionRepository permissionRepository, IUnitOfWork unitOfWork)
        {
            _permissionRepository = permissionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Permission> CreatePermissionAsync(string name, string description = null)
        {
            if (await _permissionRepository.IsPermissionNameExistsAsync(name))
                throw new InvalidOperationException($"Permission '{name}' already exists.");

            var permission = new Permission
            {
                Name = name,
                Description = description
            };

            await _permissionRepository.AddAsync(permission);
            await _unitOfWork.SaveChangesAsync();

            return permission;
        }

        public async Task<Permission> GetByIdAsync(int id)
        {
            return await _permissionRepository.GetByIdAsync(id);
        }

        public async Task<Permission> GetByNameAsync(string name)
        {
            return await _permissionRepository.GetByNameAsync(name);
        }

        public async Task<List<Permission>> GetAllPermissionsAsync()
        {
            return await _permissionRepository.GetAllAsync();
        }

        public async Task<List<Permission>> GetRolePermissionsAsync(int roleId)
        {
            return await _permissionRepository.GetRolePermissionsAsync(roleId);
        }

        public async Task<List<Permission>> GetUserPermissionsAsync(int userId)
        {
            return await _permissionRepository.GetUserPermissionsAsync(userId);
        }

        public async Task<bool> UserHasPermissionAsync(int userId, string permissionName)
        {
            return await _permissionRepository.UserHasPermissionAsync(userId, permissionName);
        }

        public async Task<bool> UserHasAnyPermissionAsync(int userId, List<string> permissionNames)
        {
            if (permissionNames == null || !permissionNames.Any())
                return false;

            foreach (var permissionName in permissionNames)
            {
                if (await _permissionRepository.UserHasPermissionAsync(userId, permissionName))
                    return true;
            }

            return false;
        }

        public async Task<bool> UserHasAllPermissionsAsync(int userId, List<string> permissionNames)
        {
            if (permissionNames == null || !permissionNames.Any())
                return true;

            foreach (var permissionName in permissionNames)
            {
                if (!await _permissionRepository.UserHasPermissionAsync(userId, permissionName))
                    return false;
            }

            return true;
        }

        public async Task<Permission> UpdatePermissionAsync(int id, string name, string description = null)
        {
            var permission = await _permissionRepository.GetByIdAsync(id);
            if (permission == null)
                throw new ArgumentException("Permission not found.");

            if (permission.Name != name && await _permissionRepository.IsPermissionNameExistsAsync(name))
                throw new InvalidOperationException($"Permission '{name}' already exists.");

            permission.Name = name;
            permission.Description = description;

            await _permissionRepository.UpdateAsync(permission);
            await _unitOfWork.SaveChangesAsync();

            return permission;
        }

        public async Task DeletePermissionAsync(int id, int deletedByUserId)
        {
            await _permissionRepository.SoftDeleteAsync(id, deletedByUserId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<Permission>> GetPermissionsByNamesAsync(List<string> permissionNames)
        {
            return await _permissionRepository.GetPermissionsByNamesAsync(permissionNames);
        }
    }
}