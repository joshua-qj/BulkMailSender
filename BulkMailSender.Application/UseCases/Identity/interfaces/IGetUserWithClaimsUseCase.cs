using BulkMailSender.Application.Dtos.UserDtos;

namespace BulkMailSender.Application.UseCases.Identity.interfaces {
    public interface IGetUserWithClaimsUseCase {
        Task<(UserDto? user, List<string> claimValues)> ExecuteAsync(string userId, string claimType);
    }
}