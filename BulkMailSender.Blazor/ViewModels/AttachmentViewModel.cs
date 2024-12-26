namespace BulkMailSender.Blazor.ViewModels {
    public class AttachmentViewModel {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public byte[] Content { get; set; }

        public override bool Equals(object obj) {
            if (obj is AttachmentViewModel other) {
                return string.Equals(FileName, other.FileName, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public override int GetHashCode() {
            return FileName != null ? FileName.GetHashCode() : 0;
        }
    }
}
