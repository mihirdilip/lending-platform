using LendingPlatform.LoanApplication;

namespace LendingPlatform.Events
{
    internal class LoanApplicationAppliedEvent : DomainEvent
    {
        /*
         * It should not be dependent on the result class as that can change anytime and so it is not loosly coupled.
         * For production, I would change it so that event only has properties that are required for this event and not depend on shared model.
         */
        public LoanApplicationAppliedEvent(LoanApplicationResult result)
        {
            Result = result;
        }

        public LoanApplicationResult Result { get; }
    }
}
