using BuildYourOwnCqrs.Data;
using BuildYourOwnCqrs.Models;

namespace BuildYourOwnCqrs
{
    public record GetTodoByIdQuery(int Id) : IQuery<Todo>;

    public class GetTodoByIdQueryHandler(TodoDbContext db)
        : IQueryHandler<GetTodoByIdQuery, Todo>
    {
        public async Task<Result<Todo>> Handle(
            GetTodoByIdQuery query,
            CancellationToken cancellationToken)
        {
            if (query.Id <= 0)
                return Result<Todo>.Failure("Invalid Todo ID");

            try
            {
                var todo = await db.Todos.FindAsync(query.Id, cancellationToken);
                if (todo is null)
                    return Result<Todo>.Failure($"Todo with ID {query.Id} not found");

                return Result<Todo>.Success(todo);
            }
            catch (Exception ex)
            {
                return Result<Todo>.Failure($"Failed to retrieve todo: {ex.Message}");
            }
        }
    }
}
