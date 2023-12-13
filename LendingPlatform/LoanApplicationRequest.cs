namespace LendingPlatform
{
    internal class LoanApplicationRequest
    {
        public LoanApplicationRequest(decimal loanAmount, decimal assetValue, int creditScore)
        {
            if (loanAmount <= 0) throw new ArgumentNullException(nameof(loanAmount));
            if (assetValue <= 0) throw new ArgumentNullException(nameof(assetValue));
            if (creditScore <= 0) throw new ArgumentNullException(nameof(creditScore));

            LoanAmount = loanAmount;
            AssetValue = assetValue;
            CreditScore = creditScore;
            LoanToValuePercentage = (loanAmount / assetValue) * 100;
        }

        public decimal LoanAmount { get; }
        public decimal AssetValue { get; }
        public int CreditScore { get; }
        public decimal LoanToValuePercentage { get; }
    }
}
