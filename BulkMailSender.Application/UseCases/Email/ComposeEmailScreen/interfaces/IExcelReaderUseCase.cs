using BulkMailSender.Application.Dtos;

namespace BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces {
    public interface IExcelReaderUseCase {
        List<ExcelImportDto> ExecuteAsync(byte[] bytes);
    }
}
