using BulkMailSender.Application.Dtos.UserDtos;

namespace BulkMailSender.Application.UseCases.Identity.interfaces {
    public interface IUpdateUserClaimsUseCase {
        Task<ResultDto> ExecuteAsync(string userId, string claimType, List<string> newClaims);
    }
}