using AutoMapper;
using BulkMailSender.Application.Interfaces.User;
using BulkMailSender.Domain.Entities.Identity;
using BulkMailSender.Infrastructure.Common.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkMailSender.Infrastructure.InMemoryPersistence.Repositories {
    public class InMemoryAuthRepository : IAuthRepository {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;

        public InMemoryAuthRepository(UserManager<ApplicationUser> userManager,
                                       SignInManager<ApplicationUser> signInManager,
                                       IMapper mapper) {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        public async Task<Result> AuthenticateAsync(string username, string password, bool rememberMe) {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null || !user.IsActive) return Result.Failure("Invalid login attempt.");

            var result = await _signInManager.PasswordSignInAsync(username, password, rememberMe, false);
            return result.Succeeded
                ? Result.Success()
                : Result.Failure("Invalid login attempt.");
        }

        public async Task<Result> SignOutAsync() {
            await _signInManager.SignOutAsync();
            return Result.Success();
        }

        public async Task<Result> ResetPasswordAsync(Guid userId, string token, string newPassword) {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return Result.Failure("User not found.");

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            return result.Succeeded
                ? Result.Success()
                : Result.Failure(result.Errors.Select(e => e.Description).ToArray());
        }

        public async Task<Result> AddPasswordAsync(Guid userId, string newPassword) {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return Result.Failure("User not found.");

            if (await _userManager.HasPasswordAsync(user))
                return Result.Failure("User already has a password.");

            var result = await _userManager.AddPasswordAsync(user, newPassword);
            return result.Succeeded
                ? Result.Success()
                : Result.Failure(result.Errors.Select(e => e.Description).ToArray());
        }
        public async Task<Result> AddUserClaimAsync(string userId, Claim newClaim) {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Result.Failure("User not found.");

            var systemClaim = _mapper.Map<System.Security.Claims.Claim>(newClaim);
            var result = await _userManager.AddClaimAsync(user, systemClaim);

            return result.Succeeded
                ? Result.Success()
                : Result.Failure(result.Errors.Select(e => e.Description).ToArray());
        }

        public async Task<Result> RemoveClaimAsync(string userId, Claim oldClaim) {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Result.Failure("User not found.");

            var systemClaim = _mapper.Map<System.Security.Claims.Claim>(oldClaim);
            var result = await _userManager.RemoveClaimAsync(user, systemClaim);

            return result.Succeeded
                ? Result.Success()
                : Result.Failure(result.Errors.Select(e => e.Description).ToArray());
        }

        public async Task<Result> ReplaceUserClaimAsync(string userId, Claim oldClaim, Claim newClaim) {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Result.Failure("User not found.");

            var oldSystemClaim = _mapper.Map<System.Security.Claims.Claim>(oldClaim);
            var newSystemClaim = _mapper.Map<System.Security.Claims.Claim>(newClaim);
            var result = await _userManager.ReplaceClaimAsync(user, oldSystemClaim, newSystemClaim);

            return result.Succeeded
                ? Result.Success()
                : Result.Failure(result.Errors.Select(e => e.Description).ToArray());
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(Guid userId) {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) throw new InvalidOperationException("User not found.");

            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(Guid userId) {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return string.Empty;

            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<IEnumerable<Claim>> GetUserClaimsAsync(string userId) {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Enumerable.Empty<Claim>();

            var claims = await _userManager.GetClaimsAsync(user);
            return _mapper.Map<IEnumerable<Claim>>(claims);
        }

        public async Task<Result> IsEmailConfirmedAsync(Guid userId) {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return Result.Failure("User not found.");

            var isConfirmed = await _userManager.IsEmailConfirmedAsync(user);
            return isConfirmed ? Result.Success() : Result.Failure("Email not confirmed.");
        }

        public async Task<Result> ChangePasswordAsync(Guid userId, string oldPassword, string newPassword) {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return Result.Failure("User not found.");

            var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            return result.Succeeded
                ? Result.Success()
                : Result.Failure(result.Errors.Select(e => e.Description).ToArray());
        }

        public async Task<bool> CheckUserHasPassword(Guid userId) {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user != null && await _userManager.HasPasswordAsync(user);
        }
    }
}