using Microsoft.Extensions.DependencyInjection;

namespace DomainLayer.Extensions;

internal static class RepositoryExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
        => services
            .AddScoped<IUserRepository, UserRepository>();
}