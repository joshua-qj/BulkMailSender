namespace BulkMailSender.Application.Dtos {
    public class EmailFailureRecord {
            public string Email { get; set; } = string.Empty; 
            public string ErrorMessage { get; set; } = string.Empty;
        }
}
