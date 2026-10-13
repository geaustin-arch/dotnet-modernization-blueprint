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
        var first = repository.CreateOnce(WorkItem.Create("First", "same-key"));
        var retry = repository.CreateOnce(WorkItem.Create("First", "same-key"));
        Assert.Equal(first.Id, retry.Id);
    }
}
