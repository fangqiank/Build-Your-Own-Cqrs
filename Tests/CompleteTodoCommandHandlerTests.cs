using BuildYourOwnCqrs;
using BuildYourOwnCqrs.Data;
using BuildYourOwnCqrs.Models;
using Microsoft.EntityFrameworkCore;

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
            var todo = new Todo { Title = "Test Todo", IsCompleted = false };
            dbContext.Todos.Add(todo);
            await dbContext.SaveChangesAsync();

            var handler = new CompleteTodoCommandHandler(dbContext);
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
            var handler = new CompleteTodoCommandHandler(dbContext);
            var command = new CompleteTodoCommand(999); // Invalid ID

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Todo with ID 999 not found", result.Error);
        }
    }
}
