namespace BulkMailSender.Domain.Entities.Identity
{
    public class Result {
        private readonly List<string> _errors = new();

        public bool IsSuccess { get; private set; }
        public IEnumerable<string> Errors => _errors;

        private Result(bool isSuccess, IEnumerable<string> errors) {
            IsSuccess = isSuccess;
            _errors.AddRange(errors);
        }

        public static Result Success() {
            return new Result(true, Enumerable.Empty<string>());
        }

        public static Result Failure(params string[] errors) {
            return new Result(false, errors);
        }

        public override string ToString() {
            return IsSuccess
                ? "Succeeded"
                : $"Failed : {string.Join(", ", Errors)}";
        }
    }
}
