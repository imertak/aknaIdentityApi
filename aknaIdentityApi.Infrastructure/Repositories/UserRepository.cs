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
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _dbSet
                .Where(u => !u.IsDeleted)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<User> GetByPhoneNumberAsync(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return null;

            return await _dbSet
                .Where(u => !u.IsDeleted)
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task<List<User>> GetUsersByRoleAsync(int roleId)
        {
            return await _dbSet
                .Include(u => u.Roles)
                .Where(u => !u.IsDeleted && u.Roles.Any(r => r.Id == roleId && !r.IsDeleted))
                .ToListAsync();
        }

        public async Task<List<User>> GetUsersWithRolesAsync()
        {
            return await _dbSet
                .Include(u => u.Roles.Where(r => !r.IsDeleted))
                .Where(u => !u.IsDeleted)
                .ToListAsync();
        }

        public async Task<User> GetUserWithRolesAsync(int userId)
        {
            return await _dbSet
                .Include(u => u.Roles.Where(r => !r.IsDeleted))
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
        }

        public async Task<User> GetUserWithRolesAndPermissionsAsync(int userId)
        {
            return await _dbSet
                .Include(u => u.Roles.Where(r => !r.IsDeleted))
                    .ThenInclude(r => r.Permissions.Where(p => !p.IsDeleted))
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return await _dbSet
                .Where(u => !u.IsDeleted)
                .AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> IsPhoneNumberExistsAsync(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            return await _dbSet
                .Where(u => !u.IsDeleted)
                .AnyAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task<List<User>> GetUsersByStatusAsync(string status)
        {
            return await _dbSet
                .Where(u => !u.IsDeleted && u.AccountStatus == status)
                .ToListAsync();
        }

        public async Task<List<User>> GetUsersByTypeAsync(string userType)
        {
            return await _dbSet
                .Where(u => !u.IsDeleted && u.UserType == userType)
                .ToListAsync();
        }

        public async Task<List<User>> SearchUsersAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetActiveAsync();

            searchTerm = searchTerm.ToLower();

            return await _dbSet
                .Where(u => !u.IsDeleted &&
                           (u.FirstName.ToLower().Contains(searchTerm) ||
                            u.LastName.ToLower().Contains(searchTerm) ||
                            u.Email.ToLower().Contains(searchTerm) ||
                            u.PhoneNumber.Contains(searchTerm)))
                .ToListAsync();
        }

        public async Task<(List<User> Users, int TotalCount)> GetUsersPagedAsync(int page, int pageSize, string searchTerm = null)
        {
            var query = _dbSet
                .Include(u => u.Roles.Where(r => !r.IsDeleted))
                .Where(u => !u.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(u => u.FirstName.ToLower().Contains(searchTerm) ||
                                        u.LastName.ToLower().Contains(searchTerm) ||
                                        u.Email.ToLower().Contains(searchTerm) ||
                                        u.PhoneNumber.Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();
            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users, totalCount);
        }

        public async Task<User> GetByIdIncludeDeletedAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }
    }
}
