using BulkMailSender.Application.Interfaces.CommonService;

namespace BulkMailSender.Infrastructure.Services {
    public class ReadFileAsBytesService : IReadFileAsBytesService {
        public async Task<byte[]> ExecuteAsync(Stream fileStream) {
            if (fileStream == null) {
                throw new ArgumentNullException(nameof(fileStream), "File stream cannot be null.");
            }

            if (!fileStream.CanRead) {
                throw new InvalidOperationException("The file stream is not readable.");
            }
            using var memoryStream = new MemoryStream();
            try {
                await fileStream.CopyToAsync(memoryStream, 81920); // Use a buffer size of 80 KB for better performance
            }
            catch (IOException ioEx) {
                // Log the error (if logging is available)
                throw new InvalidOperationException("An I/O error occurred while reading the file stream.", ioEx);
            }
            catch (Exception ex) {
                // Handle exceptions related to stream copying, e.g., log the error
                throw new InvalidOperationException("Error while copying file stream.", ex);
            }

            return memoryStream.ToArray();
        }

    }
}
