namespace LendingPlatform.Tests
{
    public class DefaultLoanApplicationValidatorTest
    {
        /*
         * More tests need to be added for all the control paths.
         */

        [Theory]
        [InlineData(0, 0, 0, false)]
        [InlineData(1, 0, 0, false)]
        [InlineData(0, 1, 0, false)]
        [InlineData(0, 0, 1, false)]
        [InlineData(1500001, 3000002, 999, false)]
        [InlineData(99000, 3000002, 999, false)]
        [InlineData(1500000, 3000002, 999, true)]
        [InlineData(1500000, 3000002, 950, true)]
        [InlineData(1500000, 3000002, 949, false)]
        public async Task ValidateAsync(decimal loanAmount, decimal assetValue, int creditScore, bool expectedResult)
        {
            var result = true;
            try
            {
                var request = new LoanApplicationRequest(loanAmount, assetValue, creditScore);
                var validator = new DefaultLoanApplicationValidator();
                await validator.ValidateAsync(request, CancellationToken.None);
            }
            catch (Exception)
            {
                result = false;
            }

            Assert.Equal(expectedResult, result);
        }
    }
}