using BulkMailSender.Application.Dtos.UserDtos;

namespace BulkMailSender.Application.UseCases.Identity.interfaces {
    public interface IGetUsersUseCase {
        Task<List<UserDto>> ExecuteAsync();
    }
}