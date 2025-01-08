using BulkMailSender.Application.Dtos;
using BulkMailSender.Application.Interfaces.CommonService;
using ClosedXML.Excel;

namespace BulkMailSender.Infrastructure.Services {
    public class ExcelFileProcessingService : IExcelFileProcessingService {
        public List<ExcelImportDto> Execute(Stream fileStream) {
            if (fileStream == null) throw new ArgumentNullException(nameof(fileStream));

            var importedData = new List<ExcelImportDto>();

            using var workbook = new XLWorkbook(fileStream);
            var worksheet = workbook.Worksheets.FirstOrDefault(ws => ws.Name.Contains("email", StringComparison.OrdinalIgnoreCase));

            if (worksheet == null)
                throw new InvalidOperationException("No worksheet with 'email' in the name was found.");

            var headerRow = worksheet.Row(1); // Assuming the first row contains headers
            int firstNameIndex = headerRow.CellsUsed().FirstOrDefault(c => c.Value.ToString() == "First Name")?.Address.ColumnNumber ?? -1;
            int lastNameIndex = headerRow.CellsUsed().FirstOrDefault(c => c.Value.ToString() == "Last Name")?.Address.ColumnNumber ?? -1;
            int emailAddressIndex = headerRow.CellsUsed().FirstOrDefault(c => c.Value.ToString() == "Email Address")?.Address.ColumnNumber ?? -1;

            if (firstNameIndex == -1 || lastNameIndex == -1 || emailAddressIndex == -1)
                throw new InvalidOperationException("One or more required columns (First Name, Last Name, Email Address) are missing.");

            foreach (var row in worksheet.RowsUsed().Skip(1)) // Skip header row
            {
                var firstName = row.Cell(firstNameIndex).GetString();
                var lastName = row.Cell(lastNameIndex).GetString();
                var emailAddress = row.Cell(emailAddressIndex).GetString();

                if (!string.IsNullOrWhiteSpace(emailAddress)) // todo Validate email,need a validation
                {
                    importedData.Add(new ExcelImportDto {
                        FirstName = firstName,
                        LastName = lastName,
                        EmailAddress = emailAddress
                    });
                }
            }
            return importedData;
        }
    }
}
