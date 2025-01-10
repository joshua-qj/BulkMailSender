using BulkMailSender.Application.Dtos.UserDtos;
using BulkMailSender.Application.Interfaces.User;
using BulkMailSender.Application.UseCases.Identity.interfaces;
using BulkMailSender.Domain.Entities.Identity;

namespace BulkMailSender.Application.UseCases.Identity {
    public class UpdateUserClaimsUseCase : IUpdateUserClaimsUseCase {
        private readonly IAuthRepository _authRepository;

        public UpdateUserClaimsUseCase(IAuthRepository authRepository) {
            _authRepository = authRepository;
        }

        public async Task<ResultDto> ExecuteAsync(string userId, string claimType, List<string> newClaims) {
            var currentClaims = await _authRepository.GetUserClaimsAsync(userId);

            var currentClaimValues = currentClaims
                .Where(claim => claim.Type == claimType)
                .Select(claim => claim.Value)
                .ToList();
            var claimsToAdd = newClaims.Except(currentClaimValues).ToList();
            var claimsToRemove = currentClaimValues.Except(newClaims).ToList();

            var errors = new List<string>();
            foreach (var claimValue in claimsToAdd) {
                Result result = await _authRepository.AddUserClaimAsync(userId, new Claim(claimType, claimValue));
                if (!result.IsSuccess) {
                    errors.Add($"Failed to add claim: {claimValue}");
                }
            }
            foreach (var claimValue in claimsToRemove) {
                Result result = await _authRepository.RemoveClaimAsync(userId, new Claim(claimType, claimValue));
                if (!result.IsSuccess) {
                    errors.Add($"Failed to remove claim: {claimValue}");
                }
            }
            if (errors.Any()) {
                return ResultDto.Failure(errors.ToArray());
            }

            return ResultDto.Success();
        }
    }
}
