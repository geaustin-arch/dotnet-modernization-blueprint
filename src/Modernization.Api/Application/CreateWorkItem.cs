using Modernization.Api.Domain;

namespace Modernization.Api.Application;

public interface IWorkItemRepository
{
    WorkItem CreateOnce(WorkItem item);
    WorkItem? Find(Guid id);
}

public sealed class CreateWorkItem(IWorkItemRepository repository)
{
    public WorkItem Execute(string title, string idempotencyKey) =>
        repository.CreateOnce(WorkItem.Create(title, idempotencyKey));
}
