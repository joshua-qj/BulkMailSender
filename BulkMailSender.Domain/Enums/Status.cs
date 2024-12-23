namespace BulkMailSender.Domain.Enums
{
    public enum Status
    {
        Ready = 0,
        Delivered = 1,
        Undelivered = 2, 
        Retrying = 3,        // The system is retrying after a failure.
        Canceled = 4,        // Sending was canceled by the user or system.
        InvalidRecipient = 5 // The email address is invalid.
    }
}
