namespace FGS.Domain.Base;

public abstract class AggregateRoot<TDomainEvent> 
    where TDomainEvent : IDomainEvent
{
    private readonly List<TDomainEvent> _uncommittedEvents = [];
    public Guid Id { get; }
    public ulong Version { get;}
    public IReadOnlyList<TDomainEvent> Events => _uncommittedEvents.AsReadOnly();
    protected const ulong NoVersion = ulong.MaxValue;

    protected AggregateRoot(Guid id, ulong version)
    {
        Id = id;
        Version = version;
    }
    
    public void CommitEvents()
    {
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