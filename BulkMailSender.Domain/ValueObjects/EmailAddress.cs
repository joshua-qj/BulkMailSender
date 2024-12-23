using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BulkMailSender.Domain.ValueObjects
{
    public class EmailAddress
    {
        private static readonly string EmailPattern =
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$"; // Simple pattern for basic email validation

        public string Value { get; }

        public EmailAddress(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email address cannot be empty.", nameof(value));

            if (!IsValidEmail(value))
                throw new ArgumentException("Invalid email address format.", nameof(value));

            Value = value;
        }

        private static bool IsValidEmail(string email)
        {
            // Match the email address against the regex pattern
            return Regex.IsMatch(email, EmailPattern, RegexOptions.IgnoreCase);
        }

        public override bool Equals(object? obj) =>
            obj is EmailAddress other && Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);

        public override int GetHashCode() => Value.GetHashCode(StringComparison.OrdinalIgnoreCase);

        public override string ToString() => Value;
    }

}
