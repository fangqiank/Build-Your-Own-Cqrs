using Microsoft.Extensions.DependencyInjection;

namespace BuildYourOwnCqrs
{
    public class CommandDispatcher(IServiceProvider serviceProvider) : ICommandDispatcher
    {
        public Task<TResult> Dispatch<TCommand, TResult>(
        TCommand command,
        CancellationToken cancellationToken = default
    ) where TCommand : ICommand<TResult>
        {
            var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResult>>();
            return handler.Handle(command, cancellationToken);
        }
    }

    public class QueryDispatcher(IServiceProvider serviceProvider) : IQueryDispatcher
    {
        public Task<TResult> Dispatch<TQuery, TResult>(
        TQuery query,
        CancellationToken cancellationToken = default
    ) where TQuery : IQuery<TResult>
        {
            var handler = serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();
            return handler.Handle(query, cancellationToken);
        }
    }
}
