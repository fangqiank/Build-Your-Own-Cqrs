using BuildYourOwnCqrs.Data;
using BuildYourOwnCqrs.Models;
using Microsoft.EntityFrameworkCore;

namespace BuildYourOwnCqrs
{
    public record GetAllTodosQuery : IQuery<IEnumerable<Todo>>;

    public class GetAllTodosQueryHandler(TodoDbContext db)
        : IQueryHandler<GetAllTodosQuery, IEnumerable<Todo>>
    {
        public async Task<Result<IEnumerable<Todo>>> Handle(
        GetAllTodosQuery query,
        CancellationToken ct
    )
        {
            var todos = await db.Todos.ToListAsync(ct);
            return Result<IEnumerable<Todo>>.Success(todos);
        }
    }
}
