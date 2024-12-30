using AutoMapper;
using BulkMailSender.Application.Dtos;
using BulkMailSender.Application.Interfaces;
using BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces;
using BulkMailSender.Domain.Entities.Email;

namespace EmailSender.UseCases.EmailCompaigns.ComposeEmailScreen
{
    public class GetRequesterByNameUseCase : IGetRequesterByNameUseCase
    {
        private readonly IEmailRepository _emailRepository;
        private readonly IMapper _mapper;

        public GetRequesterByNameUseCase(IEmailRepository emailRepository, IMapper mapper)
        {
            _emailRepository = emailRepository;
            _mapper = mapper;
        }

        public async Task<RequesterDto> ExecuteAsync(string hostName)
        {
            var requester = await _emailRepository.GetRequesterByNameAsync(hostName);
            return _mapper.Map<RequesterDto>(requester);
        }
    }
}
