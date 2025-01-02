using BulkMailSender.Application.Dtos;

namespace BulkMailSender.Application.Interfaces {
    public interface ISignalRNotificationService {
        Task NotifyEmailStatusAsync(Guid batchId, EmailStatusUpdateEventDto emailStatusUpdateDto);
    }
}