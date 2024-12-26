namespace BulkMailSender.Domain.Entities.Email {
    public class JobSummary
    {
        public Guid? BatchId { get; set; }
        public Guid EmailId { get; set; }
        public DateTime Date { get; set; }
        public string EmailFrom { get; set; }
        public string Subject { get; set; }
        public int TotalEmailsSent { get; set; }
        public int SuccessfulEmails { get; set; }
        public int FailedEmails { get; set; }
        public int PendingEmails { get; set; }
    }
}
