using BulkMailSender.Domain.Entities.Identity;

namespace BulkMailSender.Application.Interfaces.User {
    public interface IAuthRepository {
        Task<Result> AuthenticateAsync(string username, string password, bool rememberMe);
        Task<string> GenerateEmailConfirmationTokenAsync(Guid userId);
        Task<string> GeneratePasswordResetTokenAsync(Guid userId);
        Task<Result> ResetPasswordAsync(Guid userId, string token, string newPassword);
        Task<Result> ChangePasswordAsync(Guid userId, string oldPassword, string newPassword);
        Task<bool> CheckUserHasPassword(Guid userId);
        Task<Result> AddPasswordAsync(Guid userId, string newPassword);

        //Claim Management

        //Task AddClaimAsync(string userId, Claim newClaim);
        Task<Result> AddUserClaimAsync(string userId, Claim newClaim);
        Task<Result> RemoveClaimAsync(string userId, Claim oldClaim);
        Task<Result> ReplaceUserClaimAsync(string userId, Claim oldClaim, Claim newClaim);
        Task<IEnumerable<Claim>> GetUserClaimsAsync(string userId);
        Task<Result> IsEmailConfirmedAsync(Guid userId);
        Task<Result> SignOutAsync();
    }
}
