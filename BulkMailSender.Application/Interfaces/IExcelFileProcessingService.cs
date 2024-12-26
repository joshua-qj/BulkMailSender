using BulkMailSender.Application.Dtos;

namespace BulkMailSender.Application.Interfaces {
    public interface IExcelFileProcessingService {
        List<ExcelImportDto> Execute(byte[] bytes);
    }
}