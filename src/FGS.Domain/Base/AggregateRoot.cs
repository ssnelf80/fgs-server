namespace FGS.Domain.Base;

public abstract class AggregateRoot<TDomainEvent> 
    where TDomainEvent : IDomainEvent
{
    private readonly List<TDomainEvent> _uncommittedEvents = [];
    public Guid Id { get; }
    public long Version { get; private set; }
    public IReadOnlyList<TDomainEvent> Events => _uncommittedEvents.AsReadOnly();

    protected AggregateRoot(Guid id, long version)
    {
        Id = id;
        Version = version;
    }
    
    public void CommitEvents()
    {
        Version += _uncommittedEvents.Count; // todo проверить, что не будет косячить с eventStore
        _uncommittedEvents.Clear();
    }
    
    protected void EmitEvent(TDomainEvent e)
    {
        ArgumentNullException.ThrowIfNull(e);
        ApplyChanges(e);
        _uncommittedEvents.Add(e);
    }

    protected abstract void ApplyChanges(TDomainEvent e);
}