using System.Text.Json.Serialization;

namespace LendingPlatform.LoanApplication
{
    internal class LoanApplicationResult
    {
        public LoanApplicationResult(LoanApplicationRequest request)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request)); ;
        }

        public LoanApplicationRequest Request { get; }
        [JsonInclude]
        public string? ErrorMessage { get; private set; }
        [JsonIgnore]
        public Exception? Exception { get; private set; }
        public bool IsSuccessful => string.IsNullOrWhiteSpace(ErrorMessage) && Exception == null;

        public void SetFailed(string errorMessage, Exception? exception = null)
        {
            ErrorMessage = errorMessage;
            Exception = exception;
        }

        public override string ToString()
        {
            return IsSuccessful
                ? "Loan Application Status: Accepted"
                : $"Loan Application Status: Declined. {Environment.NewLine}Reason: {ErrorMessage ?? Exception?.Message}";
        }
    }
}
