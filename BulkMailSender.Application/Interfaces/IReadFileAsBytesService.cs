namespace BulkMailSender.Application.Interfaces {
    public interface IReadFileAsBytesService {
        Task<byte[]> ExecuteAsync(Stream fileStream);
    }
}
