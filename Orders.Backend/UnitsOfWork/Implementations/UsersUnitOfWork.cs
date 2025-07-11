using Microsoft.AspNetCore.Identity;
using Orders.Backend.Repositories.Interfaces;
using Orders.Backend.UnitsOfWork.Interfaces;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;

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

        public async Task CheckRoleAsync(string roleName)
        {
            await _usersRepository.CheckRoleAsync(roleName);
        }

        public async Task<User> GetUserAsync(string email)
        {
            return await _usersRepository.GetUserAsync(email);
        }

        public Task<bool> IsUserInRoleAsync(User user, string roleName)
        {
            return _usersRepository.IsUserInRoleAsync(user, roleName);
        }

        public Task<SignInResult> LoginAsync(LoginDTO login)
        {
            return _usersRepository.LoginAsync(login);
        }

        public async Task LogoutAsync()
        {
            await _usersRepository.LogoutAsync();
        }
    }
}
