using BulkMailSender.Application.Interfaces;
using BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces;
using BulkMailSender.Domain.Entities.Email;

namespace EmailSender.UseCases.EmailCompaigns.ComposeEmailScreen {
    public class FindOrAddAttachmentUseCase : IFindOrAddAttachmentUseCase
    {
        private readonly IEmailRepository _emailRepository;

        public FindOrAddAttachmentUseCase(IEmailRepository emailRepository)
        {
            _emailRepository = emailRepository;
        }

        public async Task<Attachment> ExecuteAsync(Attachment attachment)
        {
            var result = await _emailRepository.FindOrAddAttachmentAsync(attachment);
            return result;
        }
    }

}
