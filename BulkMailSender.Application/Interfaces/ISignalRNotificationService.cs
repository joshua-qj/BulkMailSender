namespace BulkMailSender.Application.Interfaces {
    public interface ISignalRNotificationService {
        Task NotifyEmailStatusAsync(Guid batchId, Guid emailId, string status, string message = null);
    }
}