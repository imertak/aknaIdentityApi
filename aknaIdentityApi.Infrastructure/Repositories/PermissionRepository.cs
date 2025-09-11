using akna_api.Infrastructure.Context;
using aknaIdentities_api.Domain.Entities;
using aknaIdentity_api.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aknaIdentity_api.Infrastructure.Repositories
{
    public class PermissionRepository : Repository<Permission>, IPermissionRepository
    {
        public PermissionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Permission> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            return await _dbSet
                .Where(p => !p.IsDeleted)
                .FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower());
        }

        public async Task<List<Permission>> GetRolePermissionsAsync(int roleId)
        {
            return await _dbSet
                .Where(p => !p.IsDeleted && p.Roles.Any(r => r.Id == roleId && !r.IsDeleted))
                .ToListAsync();
        }

        public async Task<List<Permission>> GetUserPermissionsAsync(int userId)
        {
            return await _dbSet
                .Where(p => !p.IsDeleted &&
                           p.Roles.Any(r => !r.IsDeleted &&
                                           r.Users.Any(u => u.Id == userId && !u.IsDeleted)))
                .Distinct()
                .ToListAsync();
        }

        public async Task<bool> IsPermissionNameExistsAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            return await _dbSet
                .Where(p => !p.IsDeleted)
                .AnyAsync(p => p.Name.ToLower() == name.ToLower());
        }

        public async Task<bool> UserHasPermissionAsync(int userId, string permissionName)
        {
            if (string.IsNullOrWhiteSpace(permissionName))
                return false;

            return await _dbSet
                .Where(p => !p.IsDeleted)
                .AnyAsync(p => p.Name.ToLower() == permissionName.ToLower() &&
                              p.Roles.Any(r => !r.IsDeleted &&
                                              r.Users.Any(u => u.Id == userId && !u.IsDeleted)));
        }

        public async Task<List<Permission>> GetPermissionsByNamesAsync(List<string> permissionNames)
        {
            if (permissionNames == null || !permissionNames.Any())
                return new List<Permission>();

            var lowerNames = permissionNames.Select(n => n.ToLower()).ToList();

            return await _dbSet
                .Where(p => !p.IsDeleted && lowerNames.Contains(p.Name.ToLower()))
                .ToListAsync();
        }
    }
}
