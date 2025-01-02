namespace BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces {
    public interface IReadFileAsBytesUseCase {
        Task<byte[]> ExecuteAsync(Stream fileStream);
    }
}