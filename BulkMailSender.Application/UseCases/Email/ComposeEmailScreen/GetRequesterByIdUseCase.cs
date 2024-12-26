using BulkMailSender.Application.Interfaces;
using BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces;
using BulkMailSender.Domain.Entities.Email;

namespace EmailSender.UseCases.EmailCompaigns.ComposeEmailScreen
{
    public class GetRequesterByIdUseCase : IGetRequesterByIdUseCase
    {
        private readonly IEmailRepository _emailRepository;

        public GetRequesterByIdUseCase(IEmailRepository emailRepository)
        {
            _emailRepository = emailRepository;
        }

        public async Task<Requester> GetRequesterByIdAsync(Guid requesterId)
        {
            return await _emailRepository.GetRequesterByIdAsync(requesterId);
        }
    }
}
