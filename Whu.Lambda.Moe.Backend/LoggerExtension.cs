namespace Whu.Lambda.Moe.Backend;

public static partial class LoggerExtension
{
    [LoggerMessage(0,LogLevel.Information,"{username} signed up.")]
    public static partial void LogSignup(this ILogger logger,string username);

    [LoggerMessage(1,LogLevel.Information,"{username} logged in with token {token}.")]
    public static partial void LogLogin(this ILogger logger,string username,string token);

    [LoggerMessage(2, LogLevel.Information, "{username} logged out with token {token}.")]
    public static partial void LogLogout(this ILogger logger, string username,string token);

    [LoggerMessage(3, LogLevel.Error, "Token not found, but {WhyAnError}.")]
    public static partial void LogTokenNotFound(this ILogger logger, string whyAnError);

    [LoggerMessage(4, LogLevel.Debug, "{username} passed auth with token {token}")]
    public static partial void LogAuthPassed(this ILogger logger, string username, string token);
}
