using LendingPlatform.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LendingPlatform.LoanMetrics
{
    internal class LoanApplicationAppliedHandler : INotificationHandler<LoanApplicationAppliedEvent>
    {
        private readonly ILogger<LoanApplicationAppliedHandler> _logger;
        private readonly IMediator _mediator;
        private readonly ILoanMetricsRepository _repository;

        public LoanApplicationAppliedHandler(ILogger<LoanApplicationAppliedHandler> logger, IMediator mediator, ILoanMetricsRepository repository)
        {
            _logger = logger;
            _mediator = mediator;
            _repository = repository;
        }

        public async Task Handle(LoanApplicationAppliedEvent notification, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(notification);
            ArgumentNullException.ThrowIfNull(notification.Result);
            ArgumentNullException.ThrowIfNull(notification.Result.Request);

            try
            {
                var summary = await _repository.GetSummaryAsync(cancellationToken) ?? new LoanMetricsSummary();

                if (notification.Result.IsSuccessful)
                {
                    summary.TotalApplicationsAccepted++;
                    summary.TotalAcceptedLoanAmount += notification.Result.Request.LoanAmount;
                    summary.TotalAcceptedLoanToValueInPercentage += notification.Result.Request.LoanToValuePercentage;
                }
                else
                {
                    summary.TotalApplicationsDeclined++;
                }

                await _repository.SaveSummaryAsync(summary, cancellationToken);
                await _mediator.Publish(new LoanMetricsSummaryUpdatedEvent(summary));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}
