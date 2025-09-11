// IUserService.cs
using aknaIdentities_api.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace aknaIdentity_api.Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(string email, string password, string firstName, string lastName, string phoneNumber = null, string userType = "User");
        Task<User> GetByIdAsync(int id);
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByPhoneNumberAsync(string phoneNumber);
        Task<User> GetUserWithRolesAsync(int userId);
        Task<User> GetUserWithRolesAndPermissionsAsync(int userId);
        Task<List<User>> GetAllUsersAsync();
        Task<List<User>> GetUsersWithRolesAsync();
        Task<List<User>> GetUsersByRoleAsync(int roleId);
        Task<List<User>> GetUsersByStatusAsync(string status);
        Task<List<User>> GetUsersByTypeAsync(string userType);
        Task<List<User>> SearchUsersAsync(string searchTerm);
        Task<(List<User> Users, int TotalCount)> GetUsersPagedAsync(int page, int pageSize, string searchTerm = null);
        Task<User> UpdateUserAsync(int id, string firstName = null, string lastName = null, string phoneNumber = null, string userType = null);
        Task<User> UpdateUserEmailAsync(int id, string newEmail);
        Task<User> UpdateUserPasswordAsync(int id, string newPassword);
        Task<User> UpdateUserStatusAsync(int id, string status);
        Task<User> UpdateEmailConfirmationAsync(int id, bool isConfirmed);
        Task<User> UpdatePhoneConfirmationAsync(int id, bool isConfirmed);
        Task<bool> ValidatePasswordAsync(string password, string hashedPassword);
        Task<string> HashPasswordAsync(string password);
        Task<bool> IsEmailExistsAsync(string email);
        Task<bool> IsPhoneNumberExistsAsync(string phoneNumber);
        Task DeleteUserAsync(int id, int deletedByUserId);
        Task<User> RestoreUserAsync(int id);
        Task<User> GetByIdIncludeDeletedAsync(int id);
    }
}