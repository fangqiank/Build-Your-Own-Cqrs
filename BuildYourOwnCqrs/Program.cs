using BuildYourOwnCqrs;
using BuildYourOwnCqrs.Data;
using BuildYourOwnCqrs.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();

builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseSqlite("Data Source=todos.db"));
builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();
builder.Services.AddScoped<IQueryDispatcher, QueryDispatcher>();

builder.Services.AddScoped<
    ICommandHandler<CreateTodoCommand, Todo>,
    CreateTodoCommandHandler
>();
builder.Services.AddScoped<
    IQueryHandler<GetAllTodosQuery, IEnumerable<Todo>>,
    GetAllTodosQueryHandler
>();
builder.Services.AddScoped<
    ICommandHandler<CompleteTodoCommand>,
    CompleteTodoCommandHandler
>();
builder.Services.AddScoped<
    ICommandHandler<DeleteTodoCommand, Result>, 
    DeleteTodoCommandHandler
>();
builder.Services.AddScoped<
    IQueryHandler<GetTodoByIdQuery, Todo>,
    GetTodoByIdQueryHandler
>();


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
    db.Database.EnsureCreated();
    TodoDbContext.Seed(db);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Endpoints
app.MapPost("/todos", async (
    CreateTodoCommand command,
    ICommandDispatcher dispatcher
) =>
{
    var result = await dispatcher.Dispatch<CreateTodoCommand, Todo>(command);

    if (result.IsSuccess)
    {
        var todo = result.Value;
        return Results.Created($"/todos/{todo.Id}", todo);
    }

    return Results.BadRequest(new { error = result.Error });
});

app.MapGet("/todos", async (IQueryDispatcher dispatcher) =>
{
    var result = await dispatcher.Dispatch<GetAllTodosQuery, IEnumerable<Todo>>(new GetAllTodosQuery());

    if (result.IsSuccess && result.Value is not null)
    {
        return Results.Ok(result.Value); // ✅ Return the list
    }

    return Results.BadRequest(new { error = result.Error });
});

app.MapPut("/todos/{id}/complete", async (
    int id,
    ICommandDispatcher dispatcher
) =>
{
    var result = await dispatcher.Dispatch<CompleteTodoCommand>(new CompleteTodoCommand(id));
    return result.IsSuccess
        ? Results.NoContent()
        : Results.BadRequest(result.Error);
});

// Delete Todo endpoint
app.MapDelete("/todos/{id}", async (
    int id,
    ICommandDispatcher dispatcher
) =>
{
    var result = await dispatcher.Dispatch<DeleteTodoCommand, Result>(new DeleteTodoCommand(id));
    return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
});

// Get Todo by ID endpoint
app.MapGet("/todos/{id}", async (
    int id,
    IQueryDispatcher dispatcher
) =>
{
    var result = await dispatcher.Dispatch<GetTodoByIdQuery, Todo>(new GetTodoByIdQuery(id));

    return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Error);
});

app.Run();


