using BuildYourOwnCqrs.Data;
using BuildYourOwnCqrs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BuildYourOwnCqrs
{
    public record GetAllTodosQuery : IQuery<IEnumerable<Todo>>;

    public class GetAllTodosQueryHandler(
        TodoDbContext db, 
        IMemoryCache cache, 
        ILogger<GetAllTodosQueryHandler> logger
    ) : IQueryHandler<GetAllTodosQuery, IEnumerable<Todo>>
    {
        public async Task<Result<IEnumerable<Todo>>> Handle(
            GetAllTodosQuery query,
            CancellationToken ct
        )
        {
            const string cacheKey = "all_todos";

            // 尝试从缓存获取
            if (cache.TryGetValue(cacheKey, out IEnumerable<Todo> cachedTodos))
            {
                // 验证缓存数据有效性
                if (cachedTodos != null)
                {
                    logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
                    return Result<IEnumerable<Todo>>.Success(cachedTodos);
                }
                logger.LogWarning("Cached value is null for key: {CacheKey}", cacheKey);
            }

            try
            {
                // 从数据库查询
                var todos = await db.Todos.ToListAsync(ct);
                // 确保非 null（如数据库无数据则返回空集合）
                todos ??= Enumerable.Empty<Todo>().ToList();

                // 缓存结果
                cache.Set(cacheKey, todos, TimeSpan.FromMinutes(5));
                logger.LogInformation("Data retrieved from database and cached. Count: {Count}", todos.Count());

                return Result<IEnumerable<Todo>>.Success(todos);
            }
            catch (DbUpdateException ex) // 明确捕获数据库异常
            {
                logger.LogError(ex, "数据库查询失败");

                return Result<IEnumerable<Todo>>.Failure($"数据库错误: {ex.Message}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "未知错误");
                return Result<IEnumerable<Todo>>.Failure("未知错误");
            }
        }
    }
}
