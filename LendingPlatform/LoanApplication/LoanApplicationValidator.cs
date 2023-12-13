namespace LendingPlatform.LoanApplication
{
    internal interface ILoanApplicationValidator
    {
        /// <summary>
        /// Validates the loan application request. Any errors are thrown as <see cref="LoanApplicationException"/>.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        /// <exception cref="LoanApplicationException"></exception>
        Task ValidateAsync(LoanApplicationRequest request, CancellationToken cancellation);
    }

    /// <summary>
    /// Validates the loan application request using the default specification. Any errors are thrown as <see cref="LoanApplicationException"/>.
    /// </summary>
    internal class DefaultLoanApplicationValidator : ILoanApplicationValidator
    {
        public Task ValidateAsync(LoanApplicationRequest request, CancellationToken cancellation)
        {
            /*
             * The credit score of the applicant must be between 1 and 999
             * If the value of the loan is more than £1.5 million or less than £100,000 then the application must be declined
             * If the value of the loan is £1 million or more then the LTV must be 60% or less and the credit score of the applicant must be 950 or more
             * If the value of the loan is less than £1 million then the following rules apply:
             *      If the LTV is less than 60%, the credit score of the applicant must be 750 or more
             *      If the LTV is less than 80%, the credit score of the applicant must be 800 or more
             *      If the LTV is less than 90%, the credit score of the applicant must be 900 or more
             *      If the LTV is 90% or more, the application must be declined
             */

            // Below error messages can be extracted into string contants and/or resource strings for multi language support.

            ArgumentNullException.ThrowIfNull(request);

            if (request.LoanAmount < 100000 || request.LoanAmount > 1500000)
                throw new LoanApplicationException("The value of the loan must be between £100,000 and £1,500,000.");

            if (request.AssetValue < 100000)
                throw new LoanApplicationException($"The value of the asset must be greater than loan amount ({request.LoanAmount:C}).");

            if (request.CreditScore < 1 || request.CreditScore > 999)
                throw new LoanApplicationException("The credit score of the applicant must be between 1 and 999.");

            if (request.LoanAmount >= 1000000)
            {
                if (request.LoanToValuePercentage > 60)
                    throw new LoanApplicationException($"For the loan amount of {request.LoanAmount:C}, the LTV can be upto 60%. Current LTV is {request.LoanToValuePercentage:0.##}%.");

                if (request.CreditScore < 950)
                    throw new LoanApplicationException($"For the loan amount of {request.LoanAmount:C} (LTV of {request.LoanToValuePercentage:0.##}%), applicant's credit score must be at least 950.");
            }
            else
            {
                if (request.LoanToValuePercentage >= 90)
                    throw new LoanApplicationException($"For the loan amount of {request.LoanAmount:C}, the LTV can be upto 90%. Current LTV is {request.LoanToValuePercentage:0.##}%.");

                if (request.LoanToValuePercentage >= 80)
                    if (request.CreditScore < 900)
                        throw new LoanApplicationException($"For the loan amount of {request.LoanAmount:C} (LTV of {request.LoanToValuePercentage:0.##}%), applicant's credit score must be at least 900.");

                if (request.LoanToValuePercentage >= 60)
                    if (request.CreditScore < 800)
                        throw new LoanApplicationException($"For the loan amount of {request.LoanAmount:C} (LTV of {request.LoanToValuePercentage:0.##}%), applicant's credit score must be at least 800.");

                if (request.CreditScore < 750)
                    throw new LoanApplicationException($"For the loan amount of {request.LoanAmount:C} (LTV of {request.LoanToValuePercentage:0.##}%), applicant's credit score must be at least 750.");
            }

            return Task.CompletedTask;
        }
    }
}
