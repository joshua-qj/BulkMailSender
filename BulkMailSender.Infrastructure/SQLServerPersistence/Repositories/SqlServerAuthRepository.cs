using AutoMapper;
using BulkMailSender.Application.Interfaces.User;
using BulkMailSender.Domain.Entities.Identity;
using BulkMailSender.Infrastructure.Common.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace BulkMailSender.Infrastructure.SQLServerPersistence.Repositories {
    public class SqlServerAuthRepository : IAuthRepository {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;

        public SqlServerAuthRepository(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IMapper mapper) {
            _signInManager = signInManager;
            _mapper = mapper;
            _userManager = userManager;
        }
        public async Task<Result> AddPasswordAsync(Guid userId, string newPassword) {
            try {
                // Retrieve the user from the database
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null) {
                    return Result.Failure("User not found");
                }

                var hasPassword = await _userManager.HasPasswordAsync(user);
                if (hasPassword) {
                    return Result.Failure("User Password already exists");
                }

                // Add the new password
                IdentityResult result = await _userManager.AddPasswordAsync(user, newPassword);
                if (!result.Succeeded) {
                    return Result.Failure(result.Errors.Select(e => e.Description).ToArray());
                }

                return Result.Success();
            }
            catch (Exception ex) {
                // Log the exception here if needed
                return Result.Failure("An unexptected error occured while setting up password.");
            }
        }

        public async Task<Result> AddUserClaimAsync(string userId, Claim newClaim) {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null) {
                return Result.Failure("User not found");
            }
            System.Security.Claims.Claim systemClaim = _mapper.Map<System.Security.Claims.Claim>(newClaim);
            IdentityResult result = await _userManager.AddClaimAsync(user, systemClaim);
            if (!result.Succeeded) {
                return Result.Failure(result.Errors.Select(e => e.Description).ToArray());
            }
            return Result.Success();
        }
        public async Task<Result> RemoveClaimAsync(string userId, Claim oldClaim) {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null) {
                return Result.Failure("User not found");
            }
            System.Security.Claims.Claim systemClaim = _mapper.Map<System.Security.Claims.Claim>(oldClaim);
            IdentityResult result = await _userManager.RemoveClaimAsync(user, systemClaim);
            if (!result.Succeeded) {
                return Result.Failure(result.Errors.Select(e => e.Description).ToArray());
            }
            return Result.Success();
        }
        public async Task<Result> AuthenticateAsync(string username, string password, bool rememberMe) {

            var user = await _userManager.FindByNameAsync(username);
            if (user == null || !user.IsActive) {
                return Result.Failure("Invalid login attempt.");
            }

            SignInResult signInResult = await _signInManager.PasswordSignInAsync(username, password, rememberMe, lockoutOnFailure: false);
            if (signInResult.Succeeded) {
                return Result.Success();
            } else if (signInResult.IsLockedOut) {
                return Result.Failure("Account locked out.");
            }

            return Result.Failure("Invalid login attempt.");

        }
        public async Task<Result> SignOutAsync() {
            try {
                await _signInManager.SignOutAsync();
                return Result.Success();
            }
            catch (InvalidOperationException ex) {
                // Log the exception (optional)
                return Result.Failure(new[] { "Failed to sign out due to a server error." });
            }
            catch (Exception) {
                return Result.Failure(new[] { "An unexpected error occurred during sign out." });
            }
        }
        public async Task<Result> ChangePasswordAsync(Guid userId, string oldPassword, string newPassword) {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) {
                return Result.Failure("User not found.");
            }

            IdentityResult result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            if (!result.Succeeded) {
                return Result.Failure(result.Errors.Select(e => e.Description).ToArray());
            }
            await _signInManager.RefreshSignInAsync(user);
            // Return success result
            return Result.Success();

        }

        public async Task<bool> CheckUserHasPassword(Guid userId) {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) {
                return false;
            }
            var hasPassword = await _userManager.HasPasswordAsync(user);
            return hasPassword ? true : false;
        }

        public Task<Result> ConfirmEmailAsync(Guid userId, string token) {
            throw new NotImplementedException();
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(Guid userId) {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) {
                throw new InvalidOperationException("User not found.");
            }

            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(Guid userId) {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) {
                return string.Empty;
            }
            // Generate the password reset token
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<IEnumerable<Claim>> GetUserClaimsAsync(string userId) {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null) {
                return Enumerable.Empty<Claim>();
            }
            var claims = await _userManager.GetClaimsAsync(user);
            return _mapper.Map<IEnumerable<Claim>>(claims);
        }

        public async Task<Result> IsEmailConfirmedAsync(Guid userId) {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user != null) {
                var result = await _userManager.IsEmailConfirmedAsync(user);
                return result ? Result.Success() : Result.Failure("Email not confirmed.");

            }
            return Result.Failure("User not found!");
        }





        public Task<Result> ReplaceUserClaimAsync(string userId, Claim oldClaim, Claim newClaim) {
            throw new NotImplementedException();
        }

        public async Task<Result> ResetPasswordAsync(Guid userId, string token, string newPassword) {
            // Find the user by their ID
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) {
                return Result.Failure("User not found.");
            }

            // Attempt to reset the password
            IdentityResult result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded) {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result.Failure(errors);
            }

            return Result.Success();
        }


    }
}
