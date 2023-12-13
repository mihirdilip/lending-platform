using LendingPlatform.Events;
using MediatR;

namespace LendingPlatform.LoanMetrics
{
    internal class LoanMetricsSummaryUpdatedConsoleLogHandler : INotificationHandler<LoanMetricsSummaryUpdatedEvent>
    {
        public Task Handle(LoanMetricsSummaryUpdatedEvent notification, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(notification);
            ArgumentNullException.ThrowIfNull(notification.Summary);

            Console.WriteLine(notification.Summary.ToString());

            return Task.CompletedTask;
        }
    }
}
