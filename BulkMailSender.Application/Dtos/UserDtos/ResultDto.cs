namespace BulkMailSender.Application.Dtos.UserDtos {
    public class ResultDto {
        public bool IsSuccess { get; set; }
        public List<string> Errors { get; set; } = new();

        public static ResultDto Success() {
            return new ResultDto { IsSuccess = true };
        }

        public static ResultDto Failure(params string[] errors) {
            return new ResultDto { IsSuccess = false, Errors = errors.ToList() };
        }
    }
}
