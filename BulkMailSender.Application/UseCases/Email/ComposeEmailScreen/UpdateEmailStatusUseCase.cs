using BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces;
using BulkMailSender.Application.Interfaces;
using BulkMailSender.Application.Dtos;

namespace EmailSender.UseCases.EmailCompaigns.ComposeEmailScreen
{
    public class UpdateEmailStatusUseCase : IUpdateEmailStatusUseCase
    {
        private readonly IEmailRepository _emailRepository;

        public UpdateEmailStatusUseCase(IEmailRepository emailRepository)
        {
            _emailRepository = emailRepository;
        }

        public async Task ExecuteAsync(EmailDto emailDtoSave, string? errorMessage)
        {
            await _emailRepository.UpdateEmailStatusAsync(emailDtoSave, errorMessage);
        }
    }
}
