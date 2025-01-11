namespace BulkMailSender.Application.Dtos {
    public class JobSummaryDto {
        public Guid EmailId { get; set; }
        public Guid? BatchId { get; set; }
        public DateTime Date { get; set; }
        public string EmailFrom { get; set; }
        public string Subject { get; set; }
        public int TotalEmailsSent { get; set; }
        public int SuccessfulEmails { get; set; }
        public int FailedEmails { get; set; }
        public int PendingEmails { get; set; }
    }
}
