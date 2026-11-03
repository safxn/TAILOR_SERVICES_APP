using DomainLayer.Models;
using InfrastructureLayer.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace DomainLayer.Extensions;

internal static class IdentityExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>(o =>
        {
            o.Password.RequiredLength = 8;
            o.Password.RequireUppercase = true;
            o.Password.RequireLowercase = true;
            o.Password.RequireDigit = true;
            o.Password.RequireNonAlphanumeric = false;
        })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddErrorDescriber<AppIdentityErrorDescriber>()   
            .AddDefaultTokenProviders();

        return services;
    }
}