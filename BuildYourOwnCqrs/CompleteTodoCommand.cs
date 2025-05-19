using BuildYourOwnCqrs.Data;

namespace BuildYourOwnCqrs
{
    public record CompleteTodoCommand(int Id) : ICommand<Result>;

    public class CompleteTodoCommandHandler(TodoDbContext db)
        : ICommandHandler<CompleteTodoCommand, Result>
    {
        public async Task<Result> Handle(
            CompleteTodoCommand command,
            CancellationToken cancellationToken
        )
        {
            var todo = await db.Todos.FindAsync(command.Id, cancellationToken);
            if (todo is null) return Result.Failure("Todo not found");

            todo.IsCompleted = true;
            await db.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
