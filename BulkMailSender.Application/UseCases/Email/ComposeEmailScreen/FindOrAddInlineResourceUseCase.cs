using BulkMailSender.Application.Interfaces;
using BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces;
using BulkMailSender.Domain.Entities.Email;

namespace EmailSender.UseCases.EmailCompaigns.ComposeEmailScreen
{
    public class FindOrAddInlineResourceUseCase : IFindOrAddInlineResourceUseCase
    {
        private readonly IEmailRepository _emailRepository;

        public FindOrAddInlineResourceUseCase(IEmailRepository emailRepository)
        {
            _emailRepository = emailRepository;
        }

        public Task<InlineResource> ExecuteAsync(InlineResource inlineResource) {
            throw new NotImplementedException();
        }
        //public async Task<InlineResource> ExecuteAsync(InlineResource inlineResource)
        //{
        //    var result = await _emailRepository.FindOrAddInlineResourceAsync(inlineResource);
        //    return result;
        //}
    }
}
