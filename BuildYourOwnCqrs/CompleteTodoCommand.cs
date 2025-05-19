using BuildYourOwnCqrs.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BuildYourOwnCqrs
{
    public record CompleteTodoCommand(int Id) : ICommand;

    public class CompleteTodoCommandHandler(
    TodoDbContext db,
    IMemoryCache cache // 注入缓存服务
) : ICommandHandler<CompleteTodoCommand>
    {
        public async Task<Result> Handle(
            CompleteTodoCommand command,
            CancellationToken cancellationToken
        )
        {
            // 参数校验
            if (command.Id <= 0)
                return Result.Failure("无效的 Todo ID");

            try
            {
                var todo = await db.Todos.FindAsync(command.Id, cancellationToken);
                if (todo == null)
                    return Result.Failure($"未找到 ID 为 {command.Id} 的待办事项");

                // 更新状态
                todo.IsCompleted = true;
                await db.SaveChangesAsync(cancellationToken);

                // 清除相关缓存（示例键名，根据实际场景调整）
                cache.Remove("all_todos");
                cache.Remove($"todo_{command.Id}");

                return Result.Success();
            }
            catch (DbUpdateException ex)
            {
                // 记录日志或其他处理
                return Result.Failure($"标记待办事项为已完成失败: {ex.Message}");
            }
        }
    }
}
