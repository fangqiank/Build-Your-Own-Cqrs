﻿namespace BuildYourOwnCqrs
{
    public interface ICommandDispatcher
    {
        Task<Result> Dispatch<TCommand>(
            TCommand command,
            CancellationToken cancellationToken = default
        ) where TCommand : ICommand;

        Task<Result<TResponse>> Dispatch<TCommand, TResponse>(
            TCommand command,
            CancellationToken cancellationToken = default
        ) where TCommand : ICommand<TResponse>;
    }
}
