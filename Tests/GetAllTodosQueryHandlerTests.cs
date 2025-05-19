using BuildYourOwnCqrs;
using BuildYourOwnCqrs.Data;
using BuildYourOwnCqrs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests
{
    public class GetAllTodosQueryHandlerTests : IDisposable
    {
        private readonly TodoDbContext _dbContext;
        private readonly IMemoryCache _memoryCache;
        private readonly Mock<ILogger<GetAllTodosQueryHandler>> _mockLogger;

        public GetAllTodosQueryHandlerTests()
        {
            // 配置内存数据库
            var dbContextOptions = new DbContextOptionsBuilder<TodoDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_GetAllTodos")
                .Options;

            _dbContext = new TodoDbContext(dbContextOptions);

            // 初始化内存缓存
            var cacheOptions = new MemoryCacheOptions();
            _memoryCache = new MemoryCache(cacheOptions);

            // 初始化日志模拟
            _mockLogger = new Mock<ILogger<GetAllTodosQueryHandler>>();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        [Fact]
        public async Task Handle_ShouldReturnTodosFromDatabaseWhenCacheIsEmpty()
        {
            // Arrange
            _dbContext.Todos.AddRange(
                new Todo { Title = "Todo 1" },
                new Todo { Title = "Todo 2" }
            );
            await _dbContext.SaveChangesAsync();

            var handler = new GetAllTodosQueryHandler(_dbContext, _memoryCache, _mockLogger.Object);

            // Act
            var result = await handler.Handle(new GetAllTodosQuery(), CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            var todos = result.Value!.ToList();
            Assert.Equal(2, todos.Count);
            Assert.Equal("Todo 1", todos[0].Title);
            Assert.Equal("Todo 2", todos[1].Title);
        }

        [Fact]
        public async Task Handle_ShouldReturnTodosFromCacheWhenAvailable()
        {
            // Arrange
            var cachedTodos = new List<Todo>
        {
            new Todo { Title = "Cached Todo" }
        };

            // 预加载缓存
            _memoryCache.Set("all_todos", cachedTodos);

            var handler = new GetAllTodosQueryHandler(_dbContext, _memoryCache, _mockLogger.Object);

            // Act
            var result = await handler.Handle(new GetAllTodosQuery(), CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            var todos = result.Value!.ToList();
            Assert.Single(todos);
            Assert.Equal("Cached Todo", todos[0].Title);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureWhenDatabaseFails()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TodoDbContext>()
                .UseInMemoryDatabase("TestDb_Exception")
                .Options;

            using var dbContext = new TodoDbContext(options);

            // 插入非法数据（如 Title 为 null）
            dbContext.Todos.Add(new Todo { Title = null! });

            // 强制抛出异常（模拟数据库错误）
            var mockLogger = new Mock<ILogger<GetAllTodosQueryHandler>>();
            var handler = new GetAllTodosQueryHandler(
                dbContext,
                _memoryCache,
                mockLogger.Object
            );

            // Act
            var result = await handler.Handle(new GetAllTodosQuery(), CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("An error occurred while retrieving todos.", result.Error);
        }
    }
}
