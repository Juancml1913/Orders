using Microsoft.AspNetCore.Identity;
using Orders.Backend.Repositories.Interfaces;
using Orders.Backend.UnitsOfWork.Interfaces;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.UnitsOfWork.Implementations
{
    public class UsersUnitOfWork : IUsersUnitOfWork
    {
        private readonly IUsersRepository _usersRepository;

        public UsersUnitOfWork(IUsersRepository usersRepository)
        {
            _usersRepository=usersRepository;
        }
        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
            return await _usersRepository.AddUserAsync(user, password);
        }

        public async Task AddUserToRoleAsync(User user, string roleName)
        {
            await _usersRepository.AddUserToRoleAsync(user, roleName);
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            return await _usersRepository.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        public async Task CheckRoleAsync(string roleName)
        {
            await _usersRepository.CheckRoleAsync(roleName);
        }

        public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            return await _usersRepository.ConfirmEmailAsync(user, token);
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return await _usersRepository.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            return await _usersRepository.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<ActionResponse<IEnumerable<User>>> GetAsync(PaginationDTO pagination)
        {
            return await _usersRepository.GetAsync(pagination);
        }

        public async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        {
            return await _usersRepository.GetTotalPagesAsync(pagination);
        }

        public async Task<User> GetUserAsync(string email)
        {
            return await _usersRepository.GetUserAsync(email);
        }

        public async Task<User> GetUserAsync(Guid userId)
        {
            return await _usersRepository.GetUserAsync(userId);
        }

        public async Task<bool> IsUserInRoleAsync(User user, string roleName)
        {
            return await _usersRepository.IsUserInRoleAsync(user, roleName);
        }

        public async Task<SignInResult> LoginAsync(LoginDTO login)
        {
            return await _usersRepository.LoginAsync(login);
        }

        public async Task LogoutAsync()
        {
            await _usersRepository.LogoutAsync();
        }

        public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string password)
        {
            return await _usersRepository.ResetPasswordAsync(user, token, password);
        }

        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            return await _usersRepository.UpdateUserAsync(user);
        }
    }
}
