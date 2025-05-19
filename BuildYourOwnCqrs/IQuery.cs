namespace BuildYourOwnCqrs
{
    // Query marker interface
    public interface IQuery<out TResult>;

    // Query handler interface
    public interface IQueryHandler<in TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        Task<TResult> Handle(TQuery query, CancellationToken cancellationToken);
    }
}
