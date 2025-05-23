﻿using BuildYourOwnCqrs.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BuildYourOwnCqrs
{
    public record DeleteTodoCommand(int Id) : ICommand<Result>;

    public class DeleteTodoCommandHandler(TodoDbContext db, IMemoryCache cache)
     : ICommandHandler<DeleteTodoCommand, Result>
    {
        public async Task<Result<Result>> Handle(
            DeleteTodoCommand command,
            CancellationToken cancellationToken
        )
        {
            if (command.Id <= 0) return Result<Result>.Failure("Invalid Todo ID");

            try
            {
                var todo = await db.Todos.FindAsync(command.Id, cancellationToken);
                if (todo is null) 
                    return Result<Result>.Failure($"Todo with ID {command.Id} not found");

                db.Todos.Remove(todo);
                await db.SaveChangesAsync(cancellationToken);

                cache.Remove("all_todos");

                return Result<Result>.Success(Result.Success());
            }
            catch (DbUpdateException ex)
            {
                return Result<Result>.Failure($"Failed to delete todo: {ex.Message}");
            }
        }
    }
}
