using BulkMailSender.Application.Interfaces;
using BulkMailSender.Blazor.Hubs;
using BulkMailSender.Blazor.ViewModels;
using BulkMailSender.Domain.Entities.Email;
using Microsoft.AspNetCore.SignalR;

namespace BulkMailSender.Blazor.Services {
    public class SignalRNotificationService : ISignalRNotificationService {
        private readonly IHubContext<EmailStatusHub> _hubContext;

        public SignalRNotificationService(IHubContext<EmailStatusHub> hubContext) {
            _hubContext = hubContext;
        }

        public async Task NotifyEmailStatusAsync(Guid batchId, Guid emailId, string status, string message = null) {
            var connectionId = EmailStatusHub.GetConnectionIdForJob(batchId);
            if (connectionId != null) {
                var statusUpdateEvent = new EmailStatusUpdateEventViewModel {
                    // JobId = (Guid)email.JobID,
                    Status = "Completed",
                    Message = message,
                    Sent = 2,
                    //FailedEmails = failedEmails,
                    //EmailTo = email.EmailTo,
                    UpdatedAt = DateTime.UtcNow
                };
                await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveStatusUpdate", statusUpdateEvent);
            }
        }
    }
}

