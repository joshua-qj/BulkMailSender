using System.Collections.Concurrent;

namespace BulkMailSender.Application.Dtos {
    public class EmailStatusUpdateEventDto {
        public Guid JobId { get; set; }
        public string? EmailTo { get; set; }
        public string? Status { get; set; }
        //public ConcurrentDictionary<string, string>? FailedEmails { get; set; }
        //public List<EmailFailureRecord>? FailedEmails { get; set; }
        //public ConcurrentBag<EmailFailureRecord>? FailedEmails { get; set; } 
        public ConcurrentDictionary<Guid, EmailFailureRecord>? FailedEmails { get; set; }
        public int? Sent { get; set; }
        public int? Total { get; set; }
        public string? Message { get; set; }
        public DateTime? BatchStartTime { get; set; }
    }
    //            await _statusPublisher.PublishStatusUpdateAsync(status);

    /*    private readonly IEmailStatusUpdatePublisher _statusPublisher;

    public SendEmailUseCase(IEmailStatusUpdatePublisher statusPublisher)
    {
        _statusPublisher = statusPublisher;
    }

    public async Task<StatusMessage> ExecuteAsync(Email email, Guid jobId)
    {
        var status = new EmailStatusUpdateEvent
        {
            JobId = jobId,
            EmailTo = email.EmailTo,
            Status = "Processing",
            UpdatedAt = DateTime.UtcNow
        };

        await _statusPublisher.PublishStatusUpdateAsync(status);

        try
        {
            // Simulate email sending logic
            await Task.Delay(500);
            status.Status = "Success";
            status.ErrorMessage = null;
        }
        catch (Exception ex)
        {
            status.Status = "Failed";
            status.ErrorMessage = ex.Message;
        }
        finally
        {
            status.UpdatedAt = DateTime.UtcNow;
            await _statusPublisher.PublishStatusUpdateAsync(status);
        }

        return new StatusMessage { IsError = status.Status == "Failed", Message = status.ErrorMessage };
    }*/
}

