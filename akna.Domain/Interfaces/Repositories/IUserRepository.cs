using aknaIdentities_api.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aknaIdentity_api.Domain.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByPhoneNumberAsync(string phoneNumber);
        Task<List<User>> GetUsersByRoleAsync(int roleId);
        Task<List<User>> GetUsersWithRolesAsync();
        Task<User> GetUserWithRolesAsync(int userId);
        Task<User> GetUserWithRolesAndPermissionsAsync(int userId);
        Task<bool> IsEmailExistsAsync(string email);
        Task<bool> IsPhoneNumberExistsAsync(string phoneNumber);
        Task<List<User>> GetUsersByStatusAsync(string status);
        Task<List<User>> GetUsersByTypeAsync(string userType);
        Task<List<User>> SearchUsersAsync(string searchTerm);
        Task<(List<User> Users, int TotalCount)> GetUsersPagedAsync(int page, int pageSize, string searchTerm = null);
        Task<User> GetByIdIncludeDeletedAsync(int id); // Silinmiş kullanıcıları da getir
    }
}
