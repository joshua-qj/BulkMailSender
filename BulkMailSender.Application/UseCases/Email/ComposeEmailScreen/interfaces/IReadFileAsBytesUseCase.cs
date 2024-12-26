namespace BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces {
    public interface IReadFileAsBytesUseCase {
        Task<byte[]> ExcuteAsync(Stream fileStream);
    }
}