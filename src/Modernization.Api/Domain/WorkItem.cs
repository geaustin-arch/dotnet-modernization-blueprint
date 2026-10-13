namespace Modernization.Api.Domain;

public sealed record WorkItem(Guid Id, string Title, DateTimeOffset CreatedAtUtc, string IdempotencyKey)
{
    public static WorkItem Create(string title, string key)
    {
        if (string.IsNullOrWhiteSpace(title) || title.Trim().Length > 160) throw new ArgumentException("Title must contain 1 to 160 characters.");
        if (string.IsNullOrWhiteSpace(key) || key.Length > 120) throw new ArgumentException("Idempotency key must contain 1 to 120 characters.");
        return new WorkItem(Guid.NewGuid(), title.Trim(), DateTimeOffset.UtcNow, key);
    }
}
