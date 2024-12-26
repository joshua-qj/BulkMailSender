
/*EmailSender.CoreBusiness.Models;
 BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces
*/
using BulkMailSender.Domain.Entities.Email;

namespace BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces {
    public interface IFindOrAddAttachmentUseCase {
        Task<Attachment> ExecuteAsync(Attachment attachment);
    }
}