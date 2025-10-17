namespace FlexibleAuthorization.Client;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFlexibleAuthorizationClients(
        this IServiceCollection services,
        string baseUri)
    {
        if (string.IsNullOrWhiteSpace(baseUri))
            throw new ArgumentNullException(nameof(baseUri));

        List<Type> apiInterfaces = typeof(IFlexibleAuthorizationApi)
            .Assembly
            .GetTypes()
            .Where(type => typeof(IFlexibleAuthorizationApi).IsAssignableFrom(type)
                        && type.IsInterface
                        && type != typeof(IFlexibleAuthorizationApi))
            .ToList();

        foreach (Type apiInterface in apiInterfaces)
        {
            services
                .AddRefitClient(apiInterface)
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUri));
        }

        return services;
    }
}