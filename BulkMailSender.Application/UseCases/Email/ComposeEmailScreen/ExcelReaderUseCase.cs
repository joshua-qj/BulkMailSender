﻿using BulkMailSender.Application.Dtos;
using BulkMailSender.Application.Interfaces;
using BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces;

namespace BulkMailSender.Application.UseCases.Email.ComposeEmailScreen {
    public class ExcelReaderUseCase : IExcelReaderUseCase {
        private readonly IExcelFileProcessingService _excelFileProcessing;

        public ExcelReaderUseCase(IExcelFileProcessingService excelFileProcessing) {
            _excelFileProcessing = excelFileProcessing;
        }
        public List<ExcelImportDto> ExecuteAsync(byte[] bytes) {
            return _excelFileProcessing.Execute(bytes);
        }

        public List<ExcelImportDto> ExecuteAsync(Stream fileStream) {
            throw new NotImplementedException();
        }
    }
}