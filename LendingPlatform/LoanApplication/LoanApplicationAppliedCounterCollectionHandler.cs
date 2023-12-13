using LendingPlatform.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics.Metrics;

namespace LendingPlatform.LoanApplication
{
    /*
     * This is something experimental added to expose the dotnet-counters for loan applications by subscribing to the LoanApplicationApplied event.
     */
    internal class LoanApplicationAppliedCounterCollectionHandler : INotificationHandler<LoanApplicationAppliedEvent>
    {
        private readonly ILogger<LoanApplicationAppliedCounterCollectionHandler> _logger;
        private readonly DotNetCounterManager _counterManager;

        public LoanApplicationAppliedCounterCollectionHandler(ILogger<LoanApplicationAppliedCounterCollectionHandler> logger, DotNetCounterManager counterManager)
        {
            _logger = logger;
            _counterManager = counterManager;
        }

        public Task Handle(LoanApplicationAppliedEvent notification, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(notification);
            ArgumentNullException.ThrowIfNull(notification.Result);

            try
            {
                _counterManager.ApplicationsCounter.Add(1);

                if (notification.Result.IsSuccessful)
                    _counterManager.ApplicationsAcceptedCounter.Add(1);
                else
                    _counterManager.ApplicationsDeclinedCounter.Add(1);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return Task.CompletedTask;
        }
    }

    internal class DotNetCounterManager
    {
        private readonly static Meter _meter = new Meter("LoanApplicationMetrics");

        public Counter<int> ApplicationsCounter { get; } = _meter.CreateCounter<int>(
                    "applications",
                    "Loan Application",
                    "The total number of loan applications to date.");

        public Counter<int> ApplicationsAcceptedCounter { get; } = _meter.CreateCounter<int>(
                    "applications-accepted",
                    "Loan Application",
                    "The total number of loan applications accepted to date.");

        public Counter<int> ApplicationsDeclinedCounter { get; } = _meter.CreateCounter<int>(
                    "applications-declined",
                    "Loan Application",
                    "The total number of loan applications declined to date.");
    }
}
