using BulkMailSender.Blazor.Hubs;
using BulkMailSender.Application.Dtos;
using Microsoft.AspNetCore.SignalR;
using BulkMailSender.Application.Interfaces.CommonService;

namespace BulkMailSender.Blazor.Services {
    public class SignalRNotificationService : ISignalRNotificationService {
        private readonly IHubContext<EmailStatusHub> _hubContext;
        public SignalRNotificationService(IHubContext<EmailStatusHub> hubContext) {
            _hubContext = hubContext;
        }

        public async Task NotifyEmailStatusAsync(Guid batchId, EmailStatusUpdateEventDto emailStatusUpdateDto) {
            var connectionId = EmailStatusHub.GetConnectionIdForBatchJob(batchId);
            if (!string.IsNullOrEmpty(connectionId)) {
                await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveStatusUpdate", emailStatusUpdateDto);

            } else {
                Console.WriteLine("No connection found for the batch job.");
            }

        }
    }
}

