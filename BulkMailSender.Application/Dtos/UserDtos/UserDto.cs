namespace BulkMailSender.Application.Dtos.UserDtos {
    public class UserDto {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; } = true;
        public  bool EmailConfirmed { get; set; }
    }
}
