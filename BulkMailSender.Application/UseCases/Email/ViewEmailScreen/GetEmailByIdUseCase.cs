using AutoMapper;
using BulkMailSender.Application.Dtos;
using BulkMailSender.Application.Interfaces.Email;
using BulkMailSender.Application.UseCases.Email.ViewEmailScreen.Interfaces;

namespace BulkMailSender.Application.UseCases.Email.ViewEmailScreen {
    public class GetEmailByIdUseCase : IGetEmailByIdUseCase {
        private readonly IEmailRepository _emailRepository;
        private readonly IMapper _mapper;

        public GetEmailByIdUseCase(IEmailRepository emailRepository,
            IMapper mapper) {
            _emailRepository = emailRepository;
            _mapper = mapper;
        }

        public async Task<EmailDto?> ExecuteAsync(Guid emailId) {
            // Fetch the email by ID from the repository
            Domain.Entities.Email.Email? email = await _emailRepository.GetEmailByIdAsync(emailId);
            if (email == null)
                return null;
            return _mapper.Map<EmailDto>(email);
        }
    }
}