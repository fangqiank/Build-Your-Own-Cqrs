using BuildYourOwnCqrs.Models;
using Microsoft.EntityFrameworkCore;

namespace BuildYourOwnCqrs.Data
{
    public class TodoDbContext(DbContextOptions<TodoDbContext> options)
    : DbContext(options) 
    {
        public virtual DbSet<Todo> Todos => Set<Todo>();

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    => optionsBuilder.UseSqlite("Data Source=todos.db");

        public static void Seed(TodoDbContext context)
        {
            if (!context.Todos.Any())
            {
                var todos = new List<Todo>
            {
                new Todo { Title = "Buy groceries", IsCompleted = false, CreatedAt = DateTime.UtcNow.AddDays(-2) },
                new Todo { Title = "Learn .NET 9", IsCompleted = true, CreatedAt = DateTime.UtcNow.AddDays(-1) },
                new Todo { Title = "Build CQRS App", IsCompleted = false, CreatedAt = DateTime.UtcNow }
            };

                context.Todos.AddRange(todos);
                context.SaveChanges();
            }
        }
    }
}
