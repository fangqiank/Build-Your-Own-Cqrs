namespace BuildYourOwnCqrs
{
    public interface ICommandDispatcher
    {
        Task<TResult> Dispatch<TCommand, TResult>(
            TCommand command,
            CancellationToken cancellationToken = default
        ) where TCommand : ICommand<TResult>;
    }

    public interface IQueryDispatcher
    {
        Task<TResult> Dispatch<TQuery, TResult>(
            TQuery query,
            CancellationToken cancellationToken = default
        ) where TQuery : IQuery<TResult>;
    }
}
