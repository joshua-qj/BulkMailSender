using BulkMailSender.Blazor.ViewModels;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace BulkMailSender.Blazor.Hubs {
    public class EmailStatusHub : Hub {
        public async Task JoinGroup(string groupId) {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
        }

        public async Task LeaveGroup(string groupId) {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);
        }

        private static readonly ConcurrentDictionary<Guid, string> JobConnections = new ConcurrentDictionary<Guid, string>();

        public override Task OnConnectedAsync() {
            // Optionally log or handle connections
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception) {
            // Remove connection when client disconnects
            var connection = Context.ConnectionId;
            var job = JobConnections.FirstOrDefault(j => j.Value == connection);
            if (!job.Equals(default(KeyValuePair<Guid, string>))) {
                JobConnections.Remove(job.Key, out _);
            }
            return base.OnDisconnectedAsync(exception);
        }

        public Task RegisterJobConnection(Guid jobId) {
            // Store the mapping of jobId to connectionId
            JobConnections[jobId] = Context.ConnectionId;
            return Task.CompletedTask;
        }

        public static string GetConnectionIdForBatchJob(Guid batchId) {
            return JobConnections.TryGetValue(batchId, out var connectionId) ? connectionId : string.Empty;
        }
        public async Task NotifyEmailStatusAsync(string connectionId, EmailStatusUpdateEventDto statusUpdateEvent) {
            await Clients.Client(connectionId).SendAsync("ReceiveStatusUpdate", statusUpdateEvent);
        }
    }

}