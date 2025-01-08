using AutoMapper;
using BulkMailSender.Application.Dtos;
using BulkMailSender.Application.Interfaces.Email;
using BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces;

namespace BulkMailSender.Application.UseCases.Email.ComposeEmailScreen {
    public class UpdateEmailStatusUseCase : IUpdateEmailStatusUseCase {
        private readonly IEmailRepository _emailRepository;
        private readonly IMapper _mapper;

        public UpdateEmailStatusUseCase(IEmailRepository emailRepository, IMapper mapper) {
            _emailRepository = emailRepository;
            _mapper = mapper;
        }

        public async Task ExecuteAsync(EmailDto emailDtoSave, string? errorMessage) {
            BulkMailSender.Domain.Entities.Email.Email emailDomain = _mapper.Map<Domain.Entities.Email.Email>(emailDtoSave);
            await _emailRepository.UpdateEmailStatusAsync(emailDomain, errorMessage);
        }
    }
}
