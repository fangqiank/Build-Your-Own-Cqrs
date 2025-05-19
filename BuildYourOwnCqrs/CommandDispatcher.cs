namespace BuildYourOwnCqrs
{
    public class CommandDispatcher(IServiceProvider serviceProvider) : ICommandDispatcher
    {
        public async Task<Result> Dispatch<TCommand>(
            TCommand command,
            CancellationToken cancellationToken = default
        ) where TCommand : ICommand
        {
            var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();
            return await handler.Handle(command, cancellationToken);
        }

        public async Task<Result<TResponse>> Dispatch<TCommand, TResponse>(
            TCommand command,
            CancellationToken cancellationToken = default
        ) where TCommand : ICommand<TResponse>
        {
            var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResponse>>();
            return await handler.Handle(command, cancellationToken);
        }
    }
}
