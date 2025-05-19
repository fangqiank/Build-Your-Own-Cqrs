using BuildYourOwnCqrs.Data;
using BuildYourOwnCqrs.Models;

namespace BuildYourOwnCqrs
{
    public record CreateTodoCommand(string Title) : ICommand<Result<Todo>>;

    public class CreateTodoCommandHandler(TodoDbContext db)
        : ICommandHandler<CreateTodoCommand, Result<Todo>>
    {
        public async Task<Result<Todo>> Handle(
            CreateTodoCommand command,
            CancellationToken cancellationToken
        )
        {
            var todo = new Todo { Title = command.Title };
            db.Todos.Add(todo);
            await db.SaveChangesAsync(cancellationToken);
            return Result<Todo>.Success(todo);
        }
    }
}
