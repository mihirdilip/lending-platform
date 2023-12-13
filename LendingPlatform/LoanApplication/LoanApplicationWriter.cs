using LendingPlatform.LoanMetrics;
using Microsoft.Extensions.Logging;

namespace LendingPlatform.LoanApplication
{
    internal interface ILoanApplicationWriter
    {
        public Task<LoanApplicationResult> ApplyAsync(LoanApplicationRequest request, CancellationToken cancellationToken);
    }

    internal class LoanApplicationWriterForConsole : ILoanApplicationWriter
    {
        private readonly ILogger<LoanApplicationWriterForConsole> _logger;
        private readonly ILoanApplicationValidator _validator;
        private readonly ILoanApplicationRepository _applicationRepository;
        private readonly ILoanMetricsRepository _metricsRepository;

        public LoanApplicationWriterForConsole(ILogger<LoanApplicationWriterForConsole> logger, ILoanApplicationValidator validator, ILoanApplicationRepository applicationRepository, ILoanMetricsRepository metricsRepository)
        {
            _logger = logger;
            _validator = validator;
            _applicationRepository = applicationRepository;
            _metricsRepository = metricsRepository;
        }

        public async Task<LoanApplicationResult> ApplyAsync(LoanApplicationRequest request, CancellationToken cancellationToken)
        {
            var result = new LoanApplicationResult(request);
            var metricsSummary = await _metricsRepository.GetSummaryAsync(cancellationToken) ?? new LoanMetricsSummary();
            try
            {
                await _validator.ValidateAsync(request, CancellationToken.None);

                metricsSummary.TotalApplicationsAccepted++;
                metricsSummary.TotalAcceptedLoanAmount += request.LoanAmount;
                metricsSummary.TotalAcceptedLoanToValueInPercentage += request.LoanToValuePercentage;
            }
            catch (LoanApplicationException ex)
            {
                result.SetFailed(ex.Message, ex);
                metricsSummary.TotalApplicationsDeclined++;
                _logger.LogTrace(ex, ex.Message);
            }
            catch (Exception ex)
            {
                result.SetFailed(ex.Message, ex);
                _logger.LogError(ex, ex.Message);
            }

            try
            {
                await _applicationRepository.SaveAsync(result, cancellationToken);
                await _metricsRepository.SaveSummaryAsync(metricsSummary, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            Console.WriteLine(result.ToString());
            Console.WriteLine();
            Console.WriteLine(metricsSummary.ToString());
            Console.WriteLine();

            return result;
        }
    }
}
