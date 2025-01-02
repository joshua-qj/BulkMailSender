using BulkMailSender.Application.Interfaces;
using BulkMailSender.Blazor.Hubs;
using BulkMailSender.Application.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace BulkMailSender.Blazor.Services {
    public class SignalRNotificationService : ISignalRNotificationService {
        private readonly IHubContext<EmailStatusHub> _hubContext;


        public SignalRNotificationService(IHubContext<EmailStatusHub> hubContext
    ) {
            _hubContext = hubContext;

        }

        public async Task NotifyEmailStatusAsync(Guid batchId, EmailStatusUpdateEventDto emailStatusUpdateDto) {
            var connectionId = EmailStatusHub.GetConnectionIdForBatchJob(batchId);
            if (connectionId != null) {
                await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveStatusUpdate", emailStatusUpdateDto);

            };
        }
    }
}

