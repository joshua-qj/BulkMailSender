namespace BulkMailSender.Application.Interfaces.CommonService {
    public interface IReadFileAsBytesService {
        Task<byte[]> ExecuteAsync(Stream fileStream);
    }
}
