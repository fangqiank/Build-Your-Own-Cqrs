using BuildYourOwnCqrs;
using BuildYourOwnCqrs.Data;
using BuildYourOwnCqrs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Tests
{
    public class CompleteTodoCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ExistingTodoId_CompletesTodo()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TodoDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_CompleteTodo")
            .Options;

            var dbContextOptions = new DbContextOptionsBuilder<TodoDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_CompleteTodo")
                .Options;

            using var dbContext = new TodoDbContext(dbContextOptions);

            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            
            var todo = new Todo { Title = "Test Todo", IsCompleted = false };
            
            dbContext.Todos.Add(todo);
            await dbContext.SaveChangesAsync();

            var handler = new CompleteTodoCommandHandler(dbContext, memoryCache);
            var command = new CompleteTodoCommand(todo.Id);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            var updatedTodo = await dbContext.Todos.FindAsync(todo.Id);
            Assert.True(updatedTodo!.IsCompleted);
        }

        [Fact]
        public async Task Handle_NonExistentTodoId_ReturnsFailure()
        {
            // Arrange
            var dbContextOptions = new DbContextOptionsBuilder<TodoDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_CompleteTodoInvalid")
                .Options;

            using var dbContext = new TodoDbContext(dbContextOptions);

            var memoryCache = new MemoryCache(new MemoryCacheOptions());

            var handler = new CompleteTodoCommandHandler(dbContext, memoryCache);
            var command = new CompleteTodoCommand(999); // Invalid ID

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("未找到 ID 为 999 的待办事项", result.Error);
        }
    }
}
