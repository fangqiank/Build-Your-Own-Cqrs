using BuildYourOwnCqrs;
using BuildYourOwnCqrs.Data;
using Microsoft.EntityFrameworkCore;

namespace Tests
{
    public class CreateTodoCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ValidCommand_ReturnsSuccessWithTodo()
        {
            // Arrange
            var dbContextOptions = new DbContextOptionsBuilder<TodoDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_CreateTodo")
                .Options;

            using var dbContext = new TodoDbContext(dbContextOptions);
            var handler = new CreateTodoCommandHandler(dbContext);
            var command = new CreateTodoCommand("Test Todo");

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Test Todo", result.Value!.Title);
            Assert.False(result.Value.IsCompleted);
            Assert.Single(dbContext.Todos);
        }
    }
}
