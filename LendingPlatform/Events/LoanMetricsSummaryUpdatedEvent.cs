using LendingPlatform.LoanMetrics;

namespace LendingPlatform.Events
{
    internal class LoanMetricsSummaryUpdatedEvent : DomainEvent
    {
        /* 
         * It should not be dependent on the summary class as that can change anytime and so it is not loosly coupled. 
         * For production, I would change it so that event only has properties that are required for this event and not depend on shared model.
         */
        public LoanMetricsSummaryUpdatedEvent(LoanMetricsSummary summary)
        {
            Summary = summary;
        }

        public LoanMetricsSummary Summary { get; }
    }
}
