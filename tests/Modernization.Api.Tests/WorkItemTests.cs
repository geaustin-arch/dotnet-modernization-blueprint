using Modernization.Api.Domain;
using Modernization.Api.Application;
using Modernization.Api.Infrastructure;
using Xunit;

public sealed class WorkItemTests
{
    [Fact] public void RejectsBlankTitle() => Assert.Throws<ArgumentException>(() => WorkItem.Create(" ", "key-1"));
    [Fact] public void SameIdempotencyKeyReturnsOriginalItem()
    {
        var repository = new InMemoryWorkItemRepository();
        var firstItem = WorkItem.Create("First", "same-key");
        var first = repository.CreateOnce(firstItem, OutboxMessage.WorkItemCreated(firstItem));
        var retryItem = WorkItem.Create("First", "same-key");
        var retry = repository.CreateOnce(retryItem, OutboxMessage.WorkItemCreated(retryItem));
        Assert.Equal(first.Id, retry.Id);
        Assert.Single(repository.PendingMessages());
    }

    [Fact] public void SameIdempotencyKeyWithDifferentPayloadIsRejected()
    {
        var repository = new InMemoryWorkItemRepository();
        var first = WorkItem.Create("First", "same-key");
        repository.CreateOnce(first, OutboxMessage.WorkItemCreated(first));
        var retry = WorkItem.Create("Different", "same-key");
        Assert.Throws<InvalidOperationException>(() => repository.CreateOnce(retry, OutboxMessage.WorkItemCreated(retry)));
        Assert.Single(repository.PendingMessages());
    }
}
