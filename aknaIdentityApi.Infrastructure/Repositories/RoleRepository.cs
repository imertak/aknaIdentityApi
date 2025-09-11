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
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Role> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            return await _dbSet
                .Where(r => !r.IsDeleted)
                .FirstOrDefaultAsync(r => r.Name.ToLower() == name.ToLower());
        }

        public async Task<List<Role>> GetRolesWithPermissionsAsync()
        {
            return await _dbSet
                .Include(r => r.Permissions.Where(p => !p.IsDeleted))
                .Where(r => !r.IsDeleted)
                .ToListAsync();
        }

        public async Task<Role> GetRoleWithPermissionsAsync(int roleId)
        {
            return await _dbSet
                .Include(r => r.Permissions.Where(p => !p.IsDeleted))
                .FirstOrDefaultAsync(r => r.Id == roleId && !r.IsDeleted);
        }

        public async Task<List<Role>> GetUserRolesAsync(int userId)
        {
            return await _dbSet
                .Include(r => r.Permissions.Where(p => !p.IsDeleted))
                .Where(r => !r.IsDeleted && r.Users.Any(u => u.Id == userId && !u.IsDeleted))
                .ToListAsync();
        }

        public async Task<bool> IsRoleNameExistsAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            return await _dbSet
                .Where(r => !r.IsDeleted)
                .AnyAsync(r => r.Name.ToLower() == name.ToLower());
        }

        public async Task<List<Role>> GetRolesByPermissionAsync(int permissionId)
        {
            return await _dbSet
                .Include(r => r.Permissions.Where(p => !p.IsDeleted))
                .Where(r => !r.IsDeleted && r.Permissions.Any(p => p.Id == permissionId && !p.IsDeleted))
                .ToListAsync();
        }
    }
}
