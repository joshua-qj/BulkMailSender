using BulkMailSender.Domain.Entities.Email;

namespace BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces {
    public interface IFindOrAddInlineResourceUseCase
    {
        Task<InlineResource> ExecuteAsync(InlineResource inlineResource);
    }
}