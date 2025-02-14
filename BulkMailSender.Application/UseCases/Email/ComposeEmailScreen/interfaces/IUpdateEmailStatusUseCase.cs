﻿using BulkMailSender.Application.Dtos;

namespace BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces {
    public interface IUpdateEmailStatusUseCase {
        Task ExecuteAsync(EmailDto emailDtoSave, string errorMessage);
    }
}