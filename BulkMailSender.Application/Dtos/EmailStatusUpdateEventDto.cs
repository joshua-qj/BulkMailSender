using System.Collections.Concurrent;

namespace BulkMailSender.Application.Dtos {
    public class EmailStatusUpdateEventDto {
        public Guid JobId { get; set; }
        public string? EmailTo { get; set; }
        public string? Status { get; set; }
        public ConcurrentDictionary<Guid, EmailFailureRecord>? FailedEmails { get; set; }
        public int? Total { get; set; }
        public string? Message { get; set; }
        public DateTime? BatchStartTime { get; set; }
    }
 
}

