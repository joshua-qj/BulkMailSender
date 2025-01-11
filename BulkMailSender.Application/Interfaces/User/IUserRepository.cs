
using BulkMailSender.Domain.Entities.Identity;

namespace BulkMailSender.Application.Interfaces.User {
    public interface IUserRepository {
        Task<List<Domain.Entities.Identity.User>> GetAllUsersAsync();
        Task<Domain.Entities.Identity.User?> GetUserByEmailAsync(string email);
        Task<Domain.Entities.Identity.User?> GetUserByIdAsync(Guid userId);
        Task<Domain.Entities.Identity.User?> GetUserByUsernameAsync(string username);
        Task<Result> ConfirmEmailAsync(Guid userId, string token);
        Task<Result> RegisterUserAsync(Domain.Entities.Identity.User user, string password);
        Task<Result> ToggleUserActiveStatusAsync(Guid userId); // This method is not used in the application
        Task<Result> UpdateUserAsync(Domain.Entities.Identity.User user);
    }
}
