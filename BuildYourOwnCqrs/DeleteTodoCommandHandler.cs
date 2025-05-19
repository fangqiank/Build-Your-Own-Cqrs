using BuildYourOwnCqrs.Data;

namespace BuildYourOwnCqrs
{
    public record DeleteTodoCommand(int Id) : ICommand<Result>;

    public class DeleteTodoCommandHandler(TodoDbContext db)
    : ICommandHandler<DeleteTodoCommand, Result>
    {
        public async Task<Result> Handle(
            DeleteTodoCommand command,
            CancellationToken cancellationToken
        )
        {
            var todo = await db.Todos.FindAsync(command.Id, cancellationToken);
            if (todo is null) return Result.Failure("Todo not found");

            db.Todos.Remove(todo);
            await db.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
