using BulkMailSender.Application.Dtos.UserDtos;

namespace BulkMailSender.Application.UseCases.Identity.interfaces {
    public interface IConfirmEmailUserCase {
        Task<ResultDto> ExecuteAsync(Guid userId, string code);
    }
}