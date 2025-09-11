using aknaIdentities_api.Domain.Entities;
using aknaIdentity_api.Domain.Interfaces.Contracts;
using aknaIdentity_api.Domain.Interfaces.Repositories;
using aknaIdentity_api.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace aknaIdentity_api.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<User> CreateUserAsync(string email, string password, string firstName, string lastName, string phoneNumber = null, string userType = "User")
        {
            if (await _userRepository.IsEmailExistsAsync(email))
                throw new InvalidOperationException("Email already exists.");

            if (!string.IsNullOrWhiteSpace(phoneNumber) && await _userRepository.IsPhoneNumberExistsAsync(phoneNumber))
                throw new InvalidOperationException("Phone number already exists.");

            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phoneNumber,
                PasswordHash = await HashPasswordAsync(password),
                AccountStatus = "Pending",
                UserType = userType,
                IsEmailConfirmed = false,
                IsPhoneNumberConfirmed = false
            };

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return user;
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<User> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await _userRepository.GetByPhoneNumberAsync(phoneNumber);
        }

        public async Task<User> GetUserWithRolesAsync(int userId)
        {
            return await _userRepository.GetUserWithRolesAsync(userId);
        }

        public async Task<User> GetUserWithRolesAndPermissionsAsync(int userId)
        {
            return await _userRepository.GetUserWithRolesAndPermissionsAsync(userId);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<List<User>> GetUsersWithRolesAsync()
        {
            return await _userRepository.GetUsersWithRolesAsync();
        }

        public async Task<List<User>> GetUsersByRoleAsync(int roleId)
        {
            return await _userRepository.GetUsersByRoleAsync(roleId);
        }

        public async Task<List<User>> GetUsersByStatusAsync(string status)
        {
            return await _userRepository.GetUsersByStatusAsync(status);
        }

        public async Task<List<User>> GetUsersByTypeAsync(string userType)
        {
            return await _userRepository.GetUsersByTypeAsync(userType);
        }

        public async Task<List<User>> SearchUsersAsync(string searchTerm)
        {
            return await _userRepository.SearchUsersAsync(searchTerm);
        }

        public async Task<(List<User> Users, int TotalCount)> GetUsersPagedAsync(int page, int pageSize, string searchTerm = null)
        {
            return await _userRepository.GetUsersPagedAsync(page, pageSize, searchTerm);
        }

        public async Task<User> UpdateUserAsync(int id, string firstName = null, string lastName = null, string phoneNumber = null, string userType = null)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new ArgumentException("User not found.");

            if (!string.IsNullOrWhiteSpace(firstName))
                user.FirstName = firstName;

            if (!string.IsNullOrWhiteSpace(lastName))
                user.LastName = lastName;

            if (phoneNumber != null)
            {
                if (!string.IsNullOrWhiteSpace(phoneNumber) && phoneNumber != user.PhoneNumber && await _userRepository.IsPhoneNumberExistsAsync(phoneNumber))
                    throw new InvalidOperationException("Phone number already exists.");

                user.PhoneNumber = phoneNumber;
                user.IsPhoneNumberConfirmed = false;
            }

            if (!string.IsNullOrWhiteSpace(userType))
                user.UserType = userType;

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return user;
        }

        public async Task<User> UpdateUserEmailAsync(int id, string newEmail)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new ArgumentException("User not found.");

            if (newEmail != user.Email && await _userRepository.IsEmailExistsAsync(newEmail))
                throw new InvalidOperationException("Email already exists.");

            user.Email = newEmail;
            user.IsEmailConfirmed = false;

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return user;
        }

        public async Task<User> UpdateUserPasswordAsync(int id, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new ArgumentException("User not found.");

            user.PasswordHash = await HashPasswordAsync(newPassword);

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return user;
        }

        public async Task<User> UpdateUserStatusAsync(int id, string status)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new ArgumentException("User not found.");

            user.AccountStatus = status;

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return user;
        }

        public async Task<User> UpdateEmailConfirmationAsync(int id, bool isConfirmed)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new ArgumentException("User not found.");

            user.IsEmailConfirmed = isConfirmed;

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return user;
        }

        public async Task<User> UpdatePhoneConfirmationAsync(int id, bool isConfirmed)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new ArgumentException("User not found.");

            user.IsPhoneNumberConfirmed = isConfirmed;

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return user;
        }

        public async Task<bool> ValidatePasswordAsync(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        public async Task<string> HashPasswordAsync(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _userRepository.IsEmailExistsAsync(email);
        }

        public async Task<bool> IsPhoneNumberExistsAsync(string phoneNumber)
        {
            return await _userRepository.IsPhoneNumberExistsAsync(phoneNumber);
        }

        public async Task DeleteUserAsync(int id, int deletedByUserId)
        {
            await _userRepository.SoftDeleteAsync(id, deletedByUserId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<User> RestoreUserAsync(int id)
        {
            var user = await _userRepository.GetByIdIncludeDeletedAsync(id);
            if (user == null)
                throw new ArgumentException("User not found.");

            if (!user.IsDeleted)
                throw new InvalidOperationException("User is not deleted.");

            user.IsDeleted = false;
            user.DeletedAt = null;
            user.DeletedByUserId = null;

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return user;
        }

        public async Task<User> GetByIdIncludeDeletedAsync(int id)
        {
            return await _userRepository.GetByIdIncludeDeletedAsync(id);
        }
    }
}