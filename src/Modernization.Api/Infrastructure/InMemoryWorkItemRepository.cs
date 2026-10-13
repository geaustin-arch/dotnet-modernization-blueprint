using Modernization.Api.Application;
using Modernization.Api.Domain;

namespace Modernization.Api.Infrastructure;

public sealed class InMemoryWorkItemRepository : IWorkItemRepository
{
    private readonly object _gate = new();
    private readonly Dictionary<Guid, WorkItem> _items = new();
    private readonly Dictionary<string, Guid> _keys = new(StringComparer.Ordinal);
    private readonly List<OutboxMessage> _outbox = new();
    public WorkItem CreateOnce(WorkItem item, OutboxMessage message)
    {
        lock (_gate)
        {
            if (_keys.TryGetValue(item.IdempotencyKey, out var existingId)) return _items[existingId];
            if (message.AggregateId != item.Id) throw new ArgumentException("Outbox event must reference the created work item.");
            _items.Add(item.Id, item);
            _keys.Add(item.IdempotencyKey, item.Id);
            _outbox.Add(message);
            return item;
        }
    }
    public WorkItem? Find(Guid id)
    {
        lock (_gate) return _items.GetValueOrDefault(id);
    }
    public IReadOnlyCollection<OutboxMessage> PendingMessages()
    {
        lock (_gate) return _outbox.ToArray();
    }
}
