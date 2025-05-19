namespace BuildYourOwnCqrs
{
    public interface IBaseCommand;
    // Commands
    public interface ICommand: IBaseCommand;

    public interface ICommand<TResponse> : IBaseCommand;
}
