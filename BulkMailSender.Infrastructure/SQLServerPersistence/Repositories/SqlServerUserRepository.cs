using AutoMapper;
using BulkMailSender.Application.Interfaces.User;
using BulkMailSender.Domain.Entities.Identity;
using BulkMailSender.Infrastructure.Common.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BulkMailSender.Infrastructure.SQLServerPersistence.Repositories {
    public class SqlServerUserRepository : IUserRepository {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;

        public SqlServerUserRepository(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IMapper mapper) {
            _signInManager = signInManager;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<Result> ConfirmEmailAsync(Guid userId, string token) {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) {
                return Result.Failure("User not found.");
                throw new InvalidOperationException("User not found.");
            }
            IdentityResult identityResult = await _userManager.ConfirmEmailAsync(user, token);
            if (identityResult.Succeeded) {
                return Result.Success();
            }
            return Result.Failure(identityResult.Errors.Select(e => e.Description).ToArray());
        }

        public async Task<List<User>> GetAllUsersAsync() {
            var applicationUsers = await _userManager.Users.ToListAsync();
            if (!applicationUsers.Any()) {
                return new List<User>(); // Return an empty list if no users are found
            }
            return applicationUsers.Select(u => _mapper.Map<User>(u)).ToList();
        }

        public async Task<User?> GetUserByEmailAsync(string email) {

            var applicationUser = await _userManager.FindByEmailAsync(email);
            if (applicationUser == null) {
                return null;
            }
            return applicationUser == null ? null : _mapper.Map<User>(applicationUser);
        }
        public async Task<User?> GetUserByIdAsync(Guid userId) {
            if (userId == Guid.Empty) {
                return null;
            }
            var applicationUser = await _userManager.FindByIdAsync(userId.ToString());

            return applicationUser == null ? null : _mapper.Map<User>(applicationUser);
        }

        public async Task<User?> GetUserByUsernameAsync(string username) {
            var applicationUser = await _userManager.FindByNameAsync(username);

            return applicationUser == null ? null : _mapper.Map<User>(applicationUser);

        }
        public async Task<Result> RegisterUserAsync(User user, string password) {
            if (user == null) {
                return await Task.FromResult(Result.Failure("User cannot be null."));
            }

            if (string.IsNullOrEmpty(password)) {
                return await Task.FromResult(Result.Failure("Password cannot be null or empty."));
            }
            var identityUser = _mapper.Map<ApplicationUser>(user);
            IdentityResult createResult = await _userManager.CreateAsync(identityUser, password);
            if (createResult.Succeeded) {
                return Result.Success();
            }
            return Result.Failure(createResult.Errors.Select(e => e.Description).ToArray());
        }
        public async Task<Result> ToggleUserActiveStatusAsync(Guid userId) {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) {
                return Result.Failure("User not found. Please check the provided user ID.");
            }
            try {
                user.IsActive = !user.IsActive; // Toggle the active state
                await _userManager.UpdateAsync(user);
                return Result.Success();
            }
            catch (Exception) {
                return Result.Failure("An error occurred while updating the user's active status. Please try again later.");
            }
        }

        public async Task<Result> UpdateUserAsync(User user) {

            var applicationUser = await _userManager.FindByIdAsync(user.Id.ToString());
            if (applicationUser == null) {
                return Result.Failure("User not found.");
            }
            _mapper.Map(user, applicationUser);

            IdentityResult result = await _userManager.UpdateAsync(applicationUser);
            return result.Succeeded ? Result.Success() : Result.Failure("Failed to update user.");
        }
    }
}
