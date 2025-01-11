using BulkMailSender.Application.Dtos.UserDtos;
using BulkMailSender.Application.Interfaces.User;
using BulkMailSender.Application.UseCases.Identity.interfaces;
using BulkMailSender.Domain.Entities.Identity;

namespace BulkMailSender.Application.UseCases.Identity {
    public class ToggleUserStatusUseCase : IToggleUserStatusUseCase {
        private readonly IUserRepository _userRepository;

        public ToggleUserStatusUseCase(IUserRepository userRepository) {
            _userRepository = userRepository;
        }

        public async Task<ResultDto> ExecuteAsync(Guid userId) {
            User? user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null) {
                return ResultDto.Failure("User not found.");
            }

            try {
                // Use domain logic
                if (user.IsActive)
                    user.Deactivate(); // Domain method to handle deactivation
                else
                    user.Activate();   // Domain method to handle activation

                Result result= await _userRepository.UpdateUserAsync(user);// Persist updated state
                if (result.IsSuccess) {
                    return ResultDto.Success();
                } else {
                    return ResultDto.Failure(result.Errors.ToArray());
                }
            }
            catch (Exception) {
                return ResultDto.Failure("An error occurred while updating the user's active status. Please try again later.");
            }

            //if (user != null) // Check if user is already inactive
            //{
            //    Result result = await _userRepository.ToggleUserActiveStatusAsync(userId);
            //    if (result.IsSuccess) {
            //        return ResultDto.Success();
            //    } else {
            //        return ResultDto.Failure(result.Errors.ToArray());
            //    }
            //}
            //return ResultDto.Failure("User not found. Please check the provided user ID.");
        }
    }
}
