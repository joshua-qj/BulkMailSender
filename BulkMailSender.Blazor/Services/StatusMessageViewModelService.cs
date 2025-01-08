using BulkMailSender.Blazor.ViewModels;

namespace BulkMailSender.Blazor.Services {
    public class StatusMessageViewModelService {
        public StatusMessageViewModel CurrentStatus { get; private set; } = new StatusMessageViewModel();

        public void ResetStatusMessage() {
            CurrentStatus.Message = string.Empty;
            CurrentStatus.IsError = false;
        }

        public void SetErrorMessage(string message) {
            CurrentStatus.Message = message;
            CurrentStatus.IsError = true;
        }

        public void SetSuccessMessage(string message) {
            CurrentStatus.Message = message;
            CurrentStatus.IsError = false;
        }
    }
}
