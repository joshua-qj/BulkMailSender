namespace BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces
{
    public interface IUpdateEmailStatusUseCase
    {
        Task ExecuteAsync(Guid emailId, string? errorMessage);
    }
}