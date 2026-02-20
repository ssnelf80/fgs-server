namespace FGS.Domain.Base;

public interface IAggregateRepository<TAggregate, TDomainEvent>
    where TAggregate : AggregateRoot<TDomainEvent>
    where TDomainEvent : IDomainEvent
{
    Task<TAggregate> GetAsync(Guid Id, CancellationToken cancellationToken);
    Task<TAggregate?> GetOrDefaultAsync(Guid Id, CancellationToken cancellationToken);
    
    Task SaveAsync(TAggregate aggregate, CancellationToken cancellationToken);
}