using BulkMailSender.Application.Interfaces;
using BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces;
using BulkMailSender.Domain.Entities.Email;

namespace EmailSender.UseCases.EmailCompaigns.ComposeEmailScreen
{
    public class GetRequesterByNameUseCase : IGetRequesterByNameUseCase
    {
        private readonly IEmailRepository _emailRepository;

        public GetRequesterByNameUseCase(IEmailRepository emailRepository)
        {
            _emailRepository = emailRepository;
        }

        public async Task<Requester> ExecuteAsync(string hostName)
        {
            return await _emailRepository.GetRequesterByNameAsync(hostName);
        }
    }
}
