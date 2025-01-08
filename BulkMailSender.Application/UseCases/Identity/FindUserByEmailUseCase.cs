using AutoMapper;
using BulkMailSender.Application.Dtos.UserDtos;
using BulkMailSender.Application.Interfaces.User;
using BulkMailSender.Application.UseCases.Identity.interfaces;

namespace BulkMailSender.Application.UseCases.Identity {
    public class FindUserByEmailUseCase : IFindUserByEmailUseCase {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public FindUserByEmailUseCase(IUserRepository userRepository,
            IMapper mapper) {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserDto?> ExecuteAsync(string email) {
            if (string.IsNullOrWhiteSpace(email)) {
                return null;
            }

            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null) {
                return null;
            } else {
                return _mapper.Map<UserDto>(user);
            }
        }
    }
}
