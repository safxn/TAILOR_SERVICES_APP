using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer;
public static class LoggerExtension
{
    public static void Error(this ILogger logger, Exception ex)
    {
        logger.LogError(exception: ex, message: $"Error reported at {ex.TargetSite?.ReflectedType?.Namespace}.{ex.TargetSite?.ReflectedType?.FullName}.{ex.TargetSite?.Name}", args: ex.Message);
    }

    public static void Error(this ILogger logger, Exception ex, string message)
    {
        logger.LogError(exception: ex, message: $"Error reported at {ex.TargetSite?.ReflectedType?.Namespace}.{ex.TargetSite?.ReflectedType?.FullName}.{ex.TargetSite?.Name}", args: ex.Message);
        logger.LogError(message);
    }

    public static void Error(this ILogger logger, string message)
    {
        logger.LogError(message);
    }

    public static void Debug(this ILogger logger, string message)
    {
        if (logger.IsEnabled(LogLevel.Debug))
            logger.LogDebug(message);
    }

    public static void Information(this ILogger logger, string message)
    {
        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation(message);
    }

    public static void Warning(this ILogger logger, string message)
    {
        logger.LogWarning(message);
    }

    public static void Warning(this ILogger logger, Exception ex, string message)
    {
        logger.LogWarning(exception: ex, message: $"Error reported at {ex.TargetSite?.ReflectedType?.Namespace}.{ex.TargetSite?.ReflectedType?.FullName}.{ex.TargetSite?.Name}", args: ex.Message);
        logger.LogWarning(message);
    }
}

