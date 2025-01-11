namespace BulkMailSender.Blazor.ViewModels.UserViewModels {
    public class UserViewModel {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public string DisplayName { get; set; } // Optional for UI-specific purposes
        public bool EmailConfirmed { get; set; }
    }
}
