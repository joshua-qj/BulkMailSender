using BulkMailSender.Application.Interfaces.User;
using BulkMailSender.Application.UseCases.Identity.interfaces;
using BulkMailSender.Domain.Entities.Identity;

namespace BulkMailSender.Application.UseCases.Identity {
    public class CheckUserHasPasswordUseCase : ICheckUserHasPasswordUseCase {
        private readonly IAuthRepository _authRepository;

        public CheckUserHasPasswordUseCase(IAuthRepository authRepository) {
            _authRepository = authRepository;
        }
        public async Task<bool> ExecuteAsync(Guid userId) {
            bool result = await _authRepository.CheckUserHasPassword(userId);
            return result;
        }
    }

}
