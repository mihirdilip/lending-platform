namespace LendingPlatform
{
    internal interface ILoanApplicationWriter
    {
        public Task ApplyAsync(LoanApplicationRequest request, CancellationToken cancellation);
    }

    internal class LoanApplicationWriterForConsole : ILoanApplicationWriter
    {
        public static int _totalApplicationsAccepted = 0;
        public static int _totalApplicationsDeclined = 0;
        public static decimal _totalLoanAmount = 0;
        public static decimal _totalLoanToValue = 0;

        private readonly ILoanApplicationValidator _validator;

        public LoanApplicationWriterForConsole(ILoanApplicationValidator validator)
        {
            _validator = validator;
        }

        public async Task ApplyAsync(LoanApplicationRequest request, CancellationToken cancellation)
        {
            try
            {
                await _validator.ValidateAsync(request, CancellationToken.None);

                Console.WriteLine("Loan application was successful");
                _totalApplicationsAccepted++;
                _totalLoanAmount += request.LoanAmount;
                _totalLoanToValue += request.LoanToValuePercentage;
            }
            catch (LoanApplicationException ex)
            {
                Console.WriteLine("Loan application was unsuccessful");
                Console.WriteLine("Reason: " + ex.Message);
                _totalApplicationsDeclined++;
            }
            catch (Exception ex)
            {
                Console.WriteLine("UNKNOWN ERROR: " + ex.Message);
            }

            var meanLTV = _totalApplicationsAccepted > 0 ? _totalLoanToValue / _totalApplicationsAccepted : 0;

            Console.WriteLine();
            Console.WriteLine("Metrics Summary, ");
            Console.WriteLine($"Total declined applications: {_totalApplicationsDeclined}");
            Console.WriteLine($"Total accepted applications: {_totalApplicationsAccepted}");
            Console.WriteLine($"Total value of accepted loans: {_totalLoanAmount:C}");
            Console.WriteLine($"Mean average Loan to Value for accepted loans: {meanLTV:0.##}%");

            Console.WriteLine();
        }
    }
}
