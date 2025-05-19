using System.Diagnostics.CodeAnalysis;

namespace BuildYourOwnCqrs
{
    // Commands
    public interface ICommand<out TResult>;

    public interface ICommandHandler<in TCommand, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TResult>
        where TCommand : ICommand<TResult>
    {
        Task<TResult> Handle(TCommand command, CancellationToken cancellationToken);
    }
}
