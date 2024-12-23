namespace BulkMailSender.Domain.Entities.Identity
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; } = true;
        // Any other domain-specific properties for a user
        public User(string username, string email, string passwordHash)
        {
            Id = Guid.NewGuid();
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            IsActive = true;
        }

        // Activate or deactivate methods
        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;
    }
}
