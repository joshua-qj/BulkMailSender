using BulkMailSender.Application.Dtos;
using BulkMailSender.Domain.Entities.Email;

namespace BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces {
    public interface IGetRequesterByNameUseCase
    {
        Task<RequesterDto> ExecuteAsync(string hostName);
    }
}