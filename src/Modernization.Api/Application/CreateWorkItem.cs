using Modernization.Api.Domain;

namespace Modernization.Api.Application;

public interface IWorkItemRepository
{
    WorkItem CreateOnce(WorkItem item, OutboxMessage message);
    WorkItem? Find(Guid id);
    IReadOnlyCollection<OutboxMessage> PendingMessages();
}

public sealed class CreateWorkItem(IWorkItemRepository repository)
{
    public WorkItem Execute(string title, string idempotencyKey)
    {
        var item = WorkItem.Create(title, idempotencyKey);
        return repository.CreateOnce(item, OutboxMessage.WorkItemCreated(item));
    }
}
