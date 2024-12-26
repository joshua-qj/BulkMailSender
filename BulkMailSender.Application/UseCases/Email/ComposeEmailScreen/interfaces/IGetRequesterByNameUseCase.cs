using BulkMailSender.Domain.Entities.Email;

namespace BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces {
    public interface IGetRequesterByNameUseCase
    {
        Task<Requester> ExecuteAsync(string hostName);
    }
}