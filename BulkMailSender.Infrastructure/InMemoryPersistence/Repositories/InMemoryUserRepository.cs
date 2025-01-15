using AutoMapper;
using BulkMailSender.Application.Interfaces.User;
using BulkMailSender.Domain.Entities.Identity;
using BulkMailSender.Infrastructure.Common.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BulkMailSender.Infrastructure.InMemoryPersistence.Repositories {
    public class InMemoryUserRepository : IUserRepository {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        public InMemoryUserRepository(UserManager<ApplicationUser> userManager, IMapper mapper) {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<Result> RegisterUserAsync(User user, string password) {
            var applicationUser = new ApplicationUser {
                Id = user.Id.ToString(),
                UserName = user.Username,
                Email = user.Email,
                IsActive = user.IsActive
            };

            var result = await _userManager.CreateAsync(applicationUser, password);
            return result.Succeeded
                ? Result.Success()
                : Result.Failure(result.Errors.Select(e => e.Description).ToArray());
        }

        public async Task<User?> GetUserByIdAsync(Guid userId) {
            var applicationUser = await _userManager.FindByIdAsync(userId.ToString());
            return applicationUser == null ? null : new User {
                Id = Guid.Parse(applicationUser.Id),
                Username = applicationUser.UserName,
                Email = applicationUser.Email,
                IsActive = applicationUser.IsActive
            };
        }

        public async Task<Result> ToggleUserActiveStatusAsync(Guid userId) {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return Result.Failure("User not found.");

            user.IsActive = !user.IsActive;
            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded
                ? Result.Success()
                : Result.Failure("Failed to update user status.");
        }

        public async Task<List<User>> GetAllUsersAsync() {
            var applicationUsers = await _userManager.Users.ToListAsync();
            return applicationUsers.Select(u => new User {
                Id = Guid.Parse(u.Id),
                Username = u.UserName,
                Email = u.Email,
                IsActive = u.IsActive
            }).ToList();
        }

        public async Task<User?> GetUserByUsernameAsync(string username) {
            var applicationUser = await _userManager.FindByNameAsync(username);
            return applicationUser == null ? null : new User {
                Id = Guid.Parse(applicationUser.Id),
                Username = applicationUser.UserName,
                Email = applicationUser.Email,
                IsActive = applicationUser.IsActive
            };
        }

        public async Task<User?> GetUserByEmailAsync(string email) {
            var applicationUser = await _userManager.FindByEmailAsync(email);
            return applicationUser == null ? null : new User {
                Id = Guid.Parse(applicationUser.Id),
                Username = applicationUser.UserName,
                Email = applicationUser.Email,
                IsActive = applicationUser.IsActive
            };
        }

        public async Task<Result> ConfirmEmailAsync(Guid userId, string token) {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return Result.Failure("User not found.");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result.Succeeded
                ? Result.Success()
                : Result.Failure(result.Errors.Select(e => e.Description).ToArray());
        }

        public async Task<Result> UpdateUserAsync(User user) {
            var existingUser = await _userManager.FindByIdAsync(user.Id.ToString());
            if (existingUser == null) return Result.Failure("User not found.");

            _mapper.Map(user, existingUser);
            var result = await _userManager.UpdateAsync(existingUser);

            return result.Succeeded
                ? Result.Success()
                : Result.Failure(result.Errors.Select(e => e.Description).ToArray());
        }
    }
}