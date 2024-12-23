namespace BulkMailSender.Domain.Entities.Identity
{
    public class Result
    {
        public bool IsSuccess { get; private set; }
        public string Message { get; private set; }
        public bool RequiresTwoFactor { get; private set; }
        public bool IsLockedOut { get; private set; }
        public IEnumerable<string>? Errors { get; }
        // Constructor for successful results
        public static Result Success(string message = "") => new Result { IsSuccess = true, Message = message };


        // Constructor for failed results with a message
        public static Result Failure(string message) => new Result { IsSuccess = false, Message = message };

        // Constructor for results with additional information (e.g., requires 2FA or locked out)
        public static Result Failure(string message, bool requiresTwoFactor, bool isLockedOut)
        {
            return new Result
            {
                IsSuccess = false,
                Message = message,
                RequiresTwoFactor = requiresTwoFactor,
                IsLockedOut = isLockedOut
            };
        }
    }
}
