using Modernization.Api.Application;
using Modernization.Api.Domain;
using Modernization.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IWorkItemRepository, InMemoryWorkItemRepository>();
builder.Services.AddSingleton<CreateWorkItem>();
builder.Services.AddHealthChecks();
var app = builder.Build();
app.MapHealthChecks("/health");
app.MapPost("/work-items", (CreateWorkItemRequest request, HttpRequest http, CreateWorkItem useCase) =>
{
    var idempotencyKey = http.Headers["Idempotency-Key"].FirstOrDefault();
    if (string.IsNullOrWhiteSpace(idempotencyKey)) return Results.BadRequest(new { error = "Idempotency-Key header is required." });
    try
    {
        var item = useCase.Execute(request.Title, idempotencyKey);
        return Results.Created($"/work-items/{item.Id}", item);
    }
    catch (InvalidOperationException ex) { return Results.Conflict(new { error = ex.Message }); }
    catch (ArgumentException ex) { return Results.BadRequest(new { error = ex.Message }); }
});
app.MapGet("/work-items/{id:guid}", (Guid id, IWorkItemRepository repository) =>
    repository.Find(id) is { } item ? Results.Ok(item) : Results.NotFound());
app.Run();

public sealed record CreateWorkItemRequest(string Title);
