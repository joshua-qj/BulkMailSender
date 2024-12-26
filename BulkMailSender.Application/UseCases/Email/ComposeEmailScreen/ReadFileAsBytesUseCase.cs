using BulkMailSender.Application.Interfaces;
using BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces;


namespace EmailSender.UseCases.EmailCompaigns.ComposeEmailScreen {
    public class ReadFileAsBytesUseCase : IReadFileAsBytesUseCase
    {
        private readonly IReadFileAsBytesService _readFileAsBytes;

        public ReadFileAsBytesUseCase(IReadFileAsBytesService readFileAsBytes)
        {
            this._readFileAsBytes = readFileAsBytes;
        }

        public async Task<byte[]> ExcuteAsync(Stream fileStream) {

            return await _readFileAsBytes.ExecuteAsync(fileStream);
        }

    }
}
