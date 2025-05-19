using BuildYourOwnCqrs;
using BuildYourOwnCqrs.Data;
using BuildYourOwnCqrs.Models;
using Microsoft.EntityFrameworkCore;

namespace Tests
{
    public class GetTodoByIdQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ExistingTodoId_ReturnsTodo()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TodoDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_GetTodoById")
                .Options;

            using (var dbContext = new TodoDbContext(options))
            {
                dbContext.Todos.Add(new Todo { Title = "Test Todo" });
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new TodoDbContext(options))
            {
                var handler = new GetTodoByIdQueryHandler(dbContext);
                var query = new GetTodoByIdQuery(1); // ID=1

                // Act
                var result = await handler.Handle(query, CancellationToken.None);

                // Assert
                Assert.True(result.IsSuccess);
                Assert.Equal("Test Todo", result.Value!.Title);
            }
        }

        [Fact]
        public async Task Handle_NonExistentTodoId_ReturnsFailure()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TodoDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_GetTodoByIdInvalid")
                .Options;

            using var dbContext = new TodoDbContext(options);
            var handler = new GetTodoByIdQueryHandler(dbContext);
            var query = new GetTodoByIdQuery(999); // Invalid ID

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Todo not found", result.Error);
        }
    }

}
