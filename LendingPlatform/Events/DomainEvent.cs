using MediatR;

namespace LendingPlatform.Events
{
    internal interface IDomainEvent : INotification
    {
    }

    internal abstract class DomainEvent : IDomainEvent
    {
    }
}
