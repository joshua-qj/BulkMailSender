using BulkMailSender.Application.Dtos.UserDtos;
using BulkMailSender.Application.Interfaces.User;
using BulkMailSender.Application.UseCases.Identity.interfaces;

namespace BulkMailSender.Application.UseCases.Identity {
    public class ConfirmEmailUserCase : IConfirmEmailUserCase {
        private readonly IUserRepository _userRepository;

        public ConfirmEmailUserCase(IUserRepository userRepository) {
            _userRepository = userRepository;
        }

        public async Task<ResultDto> ExecuteAsync(Guid userId, string code) {
            try {
                // Confirm the email
                var result = await _userRepository.ConfirmEmailAsync(userId, code);
                return result.IsSuccess ? ResultDto.Success() : ResultDto.Failure("Email confirmation failed.");
            }
            catch (Exception ex) {
                // Log the error if needed
                return ResultDto.Failure("An error occurred during email confirmation.");
            }
        }
    }
}
