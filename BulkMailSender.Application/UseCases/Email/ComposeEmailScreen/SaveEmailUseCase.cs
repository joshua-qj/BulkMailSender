using AutoMapper;
using BulkMailSender.Application.Dtos;
using BulkMailSender.Application.Interfaces.Email;
using BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces;
using BulkMailSender.Domain.Entities.Email;

namespace BulkMailSender.Application.UseCases.Email.ComposeEmailScreen {
    public class SaveEmailUseCase : ISaveEmailUseCase {
        private readonly IMapper _mapper;
        private readonly IEmailRepository _emailRepo;

        public SaveEmailUseCase(IMapper mapper, IEmailRepository emailRepo) {
            _mapper = mapper;
            _emailRepo = emailRepo;
        }

        public async Task<EmailDto> ExecuteAsync(EmailDto email) {
            if (email == null) throw new ArgumentNullException(nameof(email));
            var emailDomain = _mapper.Map<BulkMailSender.Domain.Entities.Email.Email>(email);
            var result = await _emailRepo.SaveEmailAsync(emailDomain);
            return _mapper.Map<EmailDto>(emailDomain);
        }
    }
}
