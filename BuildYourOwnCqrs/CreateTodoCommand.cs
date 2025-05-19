using BuildYourOwnCqrs.Data;
using BuildYourOwnCqrs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BuildYourOwnCqrs
{
    public record CreateTodoCommand(string Title) : ICommand<Todo>;

    // 处理器（负责业务逻辑，依赖注入服务）
    public class CreateTodoCommandHandler(
        TodoDbContext db,
        IMemoryCache cache // 缓存服务注入到处理器，而非命令中
    ) : ICommandHandler<CreateTodoCommand, Todo>
    {
        public async Task<Result<Todo>> Handle(
            CreateTodoCommand command,
            CancellationToken cancellationToken
        )
        {
            // 参数校验
            if (string.IsNullOrEmpty(command.Title))
                return Result<Todo>.Failure("待办事项标题不能为空");

            try
            {
                var todo = new Todo { Title = command.Title };
                db.Todos.Add(todo);
                await db.SaveChangesAsync(cancellationToken);

                // 清除相关缓存（确保下次查询获取最新数据）
                cache.Remove("all_todos"); // 假设 "all_todos" 是缓存键
                                           // 可选：缓存新创建的 Todo（根据场景决定）
                                           // cache.Set($"todo_{todo.Id}", todo, TimeSpan.FromMinutes(5));

                return Result<Todo>.Success(todo);
            }
            catch (DbUpdateException ex)
            {
                return Result<Todo>.Failure($"创建待办事项失败: {ex.Message}");
            }
        }
    }
}
