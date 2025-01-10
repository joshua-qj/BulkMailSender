using AutoMapper;
using BulkMailSender.Application.Dtos.UserDtos;
using BulkMailSender.Application.Interfaces.User;
using BulkMailSender.Application.UseCases.Identity.interfaces;

namespace BulkMailSender.Application.UseCases.Identity {
    public class GetUsersUseCase : IGetUsersUseCase {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUsersUseCase(IUserRepository userRepository,
            IMapper mapper) {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<List<UserDto>> ExecuteAsync() {
            var domainUsers = await _userRepository.GetAllUsersAsync();
            return domainUsers.Select(u => _mapper.Map<UserDto>(u)).ToList();
        }
    }
}

