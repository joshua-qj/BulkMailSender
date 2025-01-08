using BulkMailSender.Application.Dtos;

namespace BulkMailSender.Application.Interfaces.CommonService {
    public interface ISignalRNotificationService {
        Task NotifyEmailStatusAsync(Guid batchId, EmailStatusUpdateEventDto emailStatusUpdateDto);
    }
}