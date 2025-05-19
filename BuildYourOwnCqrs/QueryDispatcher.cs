namespace BuildYourOwnCqrs
{

    public class QueryDispatcher(IServiceProvider serviceProvider, ILogger<QueryDispatcher> logger) 
        : IQueryDispatcher
    {
        public async Task<Result<TResponse>> Dispatch<TQuery, TResponse>(
            TQuery query,
            CancellationToken cancellationToken = default
        ) where TQuery : IQuery<TResponse>
        {
            logger.LogInformation("Dispatching query {QueryType}", typeof(TQuery).Name);
            try
            {
                var handler = serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResponse>>();
                var result = await handler.Handle(query, cancellationToken);
                logger.LogInformation("Query {QueryType} executed successfully", typeof(TQuery).Name);
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to dispatch query {QueryType}", typeof(TQuery).Name);
                return Result<TResponse>.Failure($"Failed to dispatch query: {ex.Message}");
            }
        }
    }
}
