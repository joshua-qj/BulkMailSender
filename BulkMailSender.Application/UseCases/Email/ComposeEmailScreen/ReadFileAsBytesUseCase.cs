using BulkMailSender.Application.Interfaces;
using BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces;


namespace BulkMailSender.Application.UseCases.Email.ComposeEmailScreen {
    public class ReadFileAsBytesUseCase : IReadFileAsBytesUseCase {
        private readonly IReadFileAsBytesService _readFileAsBytesService;

        public ReadFileAsBytesUseCase(IReadFileAsBytesService readFileAsBytes) {
            _readFileAsBytesService = readFileAsBytes;
        }

        public async Task<byte[]> ExecuteAsync(Stream fileStream) {

            return await _readFileAsBytesService.ExecuteAsync(fileStream);
        }

    }
}
