namespace Whu.Lambda.Moe.Backend;

public static partial class LoggerExtension
{
    [LoggerMessage(1, LogLevel.Information, "{id} logged in.")]
    public static partial void LogLogin(this ILogger logger, string? id);

    [LoggerMessage(2, LogLevel.Information, "{id} logged out.")]
    public static partial void LogLogout(this ILogger logger, string? id);
    [LoggerMessage(3, LogLevel.Information, "{id} logged in with {scheme}")]
    public static partial void LogLoginScheme(this ILogger logger, string? id, string scheme);

}
