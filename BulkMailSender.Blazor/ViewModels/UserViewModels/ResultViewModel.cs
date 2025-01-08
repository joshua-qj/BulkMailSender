namespace BulkMailSender.Blazor.ViewModels.UserViewModels {
    public class ResultViewModel {
        public bool IsSuccess { get; set; }
        public List<string> Errors { get; set; } = new();

        public string DisplayMessage => IsSuccess
            ? "Operation succeeded!"
            : $"Operation failed: {string.Join(", ", Errors)}";

        public static ResultViewModel Success() {
            return new ResultViewModel { IsSuccess = true };
        }

        public static ResultViewModel Failure(params string[] errors) {
            return new ResultViewModel { IsSuccess = false, Errors = errors.ToList() };
        }
    }
}
