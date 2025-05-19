using BuildYourOwnCqrs;
using BuildYourOwnCqrs.Data;
using BuildYourOwnCqrs.Models;
using Microsoft.EntityFrameworkCore;

namespace Tests
{
    public class GetAllTodosQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ReturnsAllTodos()
        {
            // Arrange
            var dbContextOptions = new DbContextOptionsBuilder<TodoDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_GetAllTodos")
                .Options;

            using var dbContext = new TodoDbContext(dbContextOptions);
            dbContext.Todos.AddRange(
                new Todo { Title = "Todo 1" },
                new Todo { Title = "Todo 2" }
            );
            await dbContext.SaveChangesAsync();

            var handler = new GetAllTodosQueryHandler(dbContext);
            var query = new GetAllTodosQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value!.Count());
        }
    }
}
