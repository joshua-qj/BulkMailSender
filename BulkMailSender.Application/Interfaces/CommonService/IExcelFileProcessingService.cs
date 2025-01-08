using BulkMailSender.Application.Dtos;

namespace BulkMailSender.Application.Interfaces.CommonService {
    public interface IExcelFileProcessingService {
        List<ExcelImportDto> Execute(Stream fileStream);
    }
}