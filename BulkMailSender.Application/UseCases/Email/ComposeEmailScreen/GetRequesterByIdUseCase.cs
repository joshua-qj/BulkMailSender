using BulkMailSender.Application.Interfaces.Email;
using BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces;
using BulkMailSender.Domain.Entities.Email;

namespace BulkMailSender.Application.UseCases.Email.ComposeEmailScreen {
    public class GetRequesterByIdUseCase : IGetRequesterByIdUseCase {
        private readonly IEmailRepository _emailRepository;

        public GetRequesterByIdUseCase(IEmailRepository emailRepository) {
            _emailRepository = emailRepository;
        }

        public async Task<Requester> GetRequesterByIdAsync(Guid requesterId) {
            return await _emailRepository.GetRequesterByIdAsync(requesterId);
        }
    }
}
