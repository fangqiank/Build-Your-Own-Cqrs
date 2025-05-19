using BuildYourOwnCqrs;
using BuildYourOwnCqrs.Data;
using BuildYourOwnCqrs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Tests
{
    public class DeleteTodoCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ExistingTodoId_DeletesTodo()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TodoDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_DeleteTodo")
                .Options;

            using (var dbContext = new TodoDbContext(options))
            {
                var todo = new Todo { Title = "Test Todo" };
                dbContext.Todos.Add(todo);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new TodoDbContext(options))
            {
                var memoryCache = new MemoryCache(new MemoryCacheOptions());
                var handler = new DeleteTodoCommandHandler(dbContext, memoryCache);
                var command = new DeleteTodoCommand(1); // Assuming the first Todo has ID=1

                // Act
                var result = await handler.Handle(command, CancellationToken.None);

                // Assert
                Assert.True(result.IsSuccess);
                var deletedTodo = await dbContext.Todos.FindAsync(1);
                Assert.Null(deletedTodo); // Verify Todo is deleted
            }
        }

        [Fact]
        public async Task Handle_NonExistentTodoId_ReturnsFailure()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TodoDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_DeleteTodoInvalid")
                .Options;

            using var dbContext = new TodoDbContext(options);
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var handler = new DeleteTodoCommandHandler(dbContext, memoryCache);
            var command = new DeleteTodoCommand(999); // Invalid ID

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Todo with ID 999 not found", result.Error);
        }
    }
}
