using System.Text.RegularExpressions;

namespace BulkMailSender.Domain.ValueObjects {
    public class EmailAddress
    {
        //private static readonly string EmailPattern =
        //    @"^[^@\s]+@[^@\s]+\.[^@\s]+$"; // Simple pattern for basic email validation
        private static readonly string EmailWithDisplayNamePattern =
            @"^[a-zA-Z\s]*<\s*[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}\s*>$";
        private static readonly string PlainEmailPattern =
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        public string Value { get; }

        public EmailAddress(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email address cannot be empty.", nameof(value));
            value = value.Trim();
            if (!IsValidEmail(value))
                throw new ArgumentException("Invalid email address format.", nameof(value));

            Value = value;
        }

        private static bool IsValidEmail(string email) {
                return Regex.IsMatch(email, EmailWithDisplayNamePattern, RegexOptions.IgnoreCase) || Regex.IsMatch(email, PlainEmailPattern, RegexOptions.IgnoreCase);

        }

        public override bool Equals(object? obj) =>
            obj is EmailAddress other && Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);

        public override int GetHashCode() => Value.GetHashCode(StringComparison.OrdinalIgnoreCase);

        public override string ToString() => Value;
    }

}
