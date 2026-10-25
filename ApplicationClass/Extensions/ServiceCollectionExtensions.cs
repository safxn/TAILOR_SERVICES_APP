using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DomainLayer.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainLayer(this IServiceCollection services, IConfiguration config)
        => services
            .AddDatabase(config)
            .AddIdentityServices()
            .AddRepositories();
}