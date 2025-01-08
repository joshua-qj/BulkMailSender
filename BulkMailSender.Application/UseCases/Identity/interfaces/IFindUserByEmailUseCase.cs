using BulkMailSender.Application.Dtos.UserDtos;

namespace BulkMailSender.Application.UseCases.Identity.interfaces {
    public interface IFindUserByEmailUseCase {
        Task<UserDto?> ExecuteAsync(string email);
    }
}