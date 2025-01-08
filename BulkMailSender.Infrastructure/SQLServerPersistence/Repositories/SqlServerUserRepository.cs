using AutoMapper;
using BulkMailSender.Application.Interfaces.User;
using BulkMailSender.Domain.Entities.Identity;
using BulkMailSender.Infrastructure.Common.Entities.Identity;
using Microsoft.AspNetCore.Identity;

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

        public Task<List<User>> GetAllUsersAsync() {
            throw new NotImplementedException();
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
        public Task<Result> ToggleUserActiveStatusAsync(Guid userId) {
            throw new NotImplementedException();
        }
    }
}
