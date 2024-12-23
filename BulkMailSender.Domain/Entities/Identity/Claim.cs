namespace BulkMailSender.Domain.Entities.Identity
{
    public class Claim
    {
        public string Type { get; private set; }
        public string Value { get; private set; }
        public Claim(string type, string value)
        {
            Type = type;
            Value = value;
        }
    }
}