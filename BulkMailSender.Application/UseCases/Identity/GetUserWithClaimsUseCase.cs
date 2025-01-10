using AutoMapper;
using BulkMailSender.Application.Dtos.UserDtos;
using BulkMailSender.Application.Interfaces.User;
using BulkMailSender.Application.UseCases.Identity.interfaces;

namespace BulkMailSender.Application.UseCases.Identity {
    public class GetUserWithClaimsUseCase : IGetUserWithClaimsUseCase {
        private readonly IUserRepository _userRepository;
        private readonly IAuthRepository _authRepository;
        private readonly IMapper _mapper;

        public GetUserWithClaimsUseCase(IUserRepository userRepository,
            IAuthRepository authRepository,
            IMapper mapper) {
            _userRepository = userRepository;
            _authRepository = authRepository;
            _mapper = mapper;
        }

        public async Task<(UserDto? user, List<string> claimValues)> ExecuteAsync(string userId, string claimType) {
            var result = Guid.TryParse(userId, out Guid userGuidId);
            if (!result) {
                return (null, new List<string>());
            }

            var user = await _userRepository.GetUserByIdAsync(userGuidId);
            if (user == null) {
                return (null, new List<string>());
            }
            var claims = await _authRepository.GetUserClaimsAsync(userId);
            //if (!claims.Any()) {
            //   // return (null, new List<string>());
            //}
            var claimValues = claims.Where(c => c.Type == claimType).Select(c => c.Value).ToList();

            var userDto = _mapper.Map<UserDto>(user);
            if (userDto == null) {
                return (null, new List<string>());
            }
            return (userDto, claimValues);
        }
    }
}
