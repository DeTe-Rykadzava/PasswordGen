using Serilog;

namespace PasswordGenApp.Services.CoreService;

public static class AppLogger
{
    private static bool _loggerInited = false;
        
    private static void InitLogger()
    {
        using var log = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("PasswordGenLibraryLogger.log", rollingInterval: RollingInterval.Month)
            .CreateLogger();
        Log.Logger = log;
        _loggerInited = true;
    }

    public static void LogError(string message, params object[] messageArgs)
    {
        if(!_loggerInited)
            InitLogger();
        Log.Logger.Error(message, messageArgs);
    }
        
    public static void LogInfo(string message, params object[] messageArgs)
    {
        if(!_loggerInited)
            InitLogger();
        Log.Logger.Information(message, messageArgs);
    }
}