using Modernization.Api.Application;
using Modernization.Api.Domain;

namespace Modernization.Api.Infrastructure;

public sealed class InMemoryWorkItemRepository : IWorkItemRepository
{
    private readonly object _gate = new();
    private readonly Dictionary<Guid, WorkItem> _items = new();
    private readonly Dictionary<string, Guid> _keys = new(StringComparer.Ordinal);
    public WorkItem CreateOnce(WorkItem item)
    {
        lock (_gate)
        {
            if (_keys.TryGetValue(item.IdempotencyKey, out var existingId)) return _items[existingId];
            _items.Add(item.Id, item);
            _keys.Add(item.IdempotencyKey, item.Id);
            return item;
        }
    }
    public WorkItem? Find(Guid id)
    {
        lock (_gate) return _items.GetValueOrDefault(id);
    }
}
