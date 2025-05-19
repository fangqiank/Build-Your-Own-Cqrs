using BuildYourOwnCqrs.Data;
using BuildYourOwnCqrs.Models;

namespace BuildYourOwnCqrs
{
    public record GetTodoByIdQuery(int Id) : IQuery<Result<Todo>>;

    public class GetTodoByIdQueryHandler(TodoDbContext db)
    : IQueryHandler<GetTodoByIdQuery, Result<Todo>>
    {
        public async Task<Result<Todo>> Handle(
            GetTodoByIdQuery query,
            CancellationToken cancellationToken
        )
        {
            var todo = await db.Todos.FindAsync(query.Id, cancellationToken);
            return todo is null
                ? Result<Todo>.Failure("Todo not found")
                : Result<Todo>.Success(todo);
        }
    }
}
