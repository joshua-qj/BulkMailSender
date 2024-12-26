using BulkMailSender.Application.Dtos;
using BulkMailSender.Application.Interfaces;
using BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces;
using BulkMailSender.Domain.Entities.Email;

namespace EmailSender.UseCases.EmailCompaigns.ComposeEmailScreen {
    public class SaveEmailUseCase : ISaveEmailUseCase
    {
        private readonly IEmailRepository _emailRepo;

        public SaveEmailUseCase(IEmailRepository emailRepo)
        {
            _emailRepo = emailRepo;
        }

        public async Task<EmailDto> ExecuteAsync(EmailDto email)
        {
            if (email == null) throw new ArgumentNullException(nameof(email));
           // var result= await _emailRepo.SaveEmailAsync(email);
            return null;
        }
    }
}
