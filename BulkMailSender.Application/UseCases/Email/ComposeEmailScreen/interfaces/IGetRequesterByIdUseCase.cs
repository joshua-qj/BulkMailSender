using BulkMailSender.Domain.Entities.Email;

namespace BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces {
    public interface IGetRequesterByIdUseCase
    {
        Task<Requester> GetRequesterByIdAsync(Guid requesterId);
    }
}