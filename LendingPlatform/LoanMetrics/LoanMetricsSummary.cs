namespace LendingPlatform.LoanMetrics
{
    internal class LoanMetricsSummary
    {
        public int TotalApplicationsAccepted { get; set; }
        public int TotalApplicationsDeclined { get; set; }
        public decimal TotalAcceptedLoanAmount { get; set; }
        public decimal TotalAcceptedLoanToValueInPercentage { get; set; }

        public int TotalApplications => TotalApplicationsAccepted + TotalApplicationsDeclined;
        public decimal MeanAverageAcceptedLoanToValueInPercentage => TotalApplicationsAccepted > 0
                                                                ? TotalAcceptedLoanToValueInPercentage / TotalApplicationsAccepted
                                                                : 0;

        public override string ToString()
        {
            return string.Format("Metrics Summary, {0}Total declined applications: {1}{0}Total accepted applications: {2}{0}Total value of accepted loans: {3:C}{0}Mean average Loan to Value for accepted loans: {4:0.##}%"
                , Environment.NewLine
                , TotalApplicationsDeclined
                , TotalApplicationsAccepted
                , TotalAcceptedLoanAmount
                , MeanAverageAcceptedLoanToValueInPercentage);
        }
    }
}
