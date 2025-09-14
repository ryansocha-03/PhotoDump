namespace photodump_api.Shared.Interfaces;

public interface IFeatureHandler<TRequest, TResponse>
{
    public Task<TResponse> HandleAsync(TRequest request);
}