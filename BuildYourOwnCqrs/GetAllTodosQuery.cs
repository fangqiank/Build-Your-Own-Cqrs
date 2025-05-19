using BuildYourOwnCqrs.Data;
using BuildYourOwnCqrs.Models;
using Microsoft.EntityFrameworkCore;

namespace BuildYourOwnCqrs
{
    public record GetAllTodosQuery : IQuery<Result<IEnumerable<Todo>>>;

    public class GetAllTodosQueryHandler(TodoDbContext db)
        : IQueryHandler<GetAllTodosQuery, Result<IEnumerable<Todo>>>
    {
        public async Task<Result<IEnumerable<Todo>>> Handle(
            GetAllTodosQuery query,
            CancellationToken cancellationToken
        )
        {
            var todos = await db.Todos.ToListAsync(cancellationToken);
            return Result<IEnumerable<Todo>>.Success(todos);
        }
    }
}
