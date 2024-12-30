namespace BulkMailSender.Domain.Enums
{
    public enum Status
    {
        Ready = 1,
        Delivered = 2,
        Undelivered = 3, 
        Retrying = 4,        // The system is retrying after a failure.
        Canceled = 5,        // Sending was canceled by the user or system.
        InvalidRecipient = 6 // The email address is invalid.
    }
}
