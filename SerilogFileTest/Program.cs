using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Display;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace SerilogFileTest;

class Program
{
    static void Main(string[] args)
    {
        // I want the log files to be created in “./.Logs”, which is a subpath of the directory where this program is running.
        // However, without Line 29, the logs are created in “./”.
        // Also, without Line 29, if I run it as `./Logs`, Logs behaves as if it were a prefix.
        var logPath = "./Logs";

        var logger = CreateLogger<Program>(LogEventLevel.Information, logPath);
        LogInformation(logger, "Hello, World!", null);
    }

    static ILogger<T> CreateLogger<T>(LogEventLevel minLogLevel, string logPath)
    {
        var outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}";
        var formatter = new MessageTemplateTextFormatter(outputTemplate, null);
        var loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Is(minLogLevel)
            .WriteTo.Console(formatter);

        var logFile = logPath;
        // logFile = Path.Combine(logPath, "log.txt");

        Log.Logger = loggerConfiguration.WriteTo
            .File(formatter, logFile, rollingInterval: RollingInterval.Day)
            .CreateLogger();

        var loggerFactory = new LoggerFactory().AddSerilog();
        return loggerFactory.CreateLogger<T>();
    }

    private static readonly Action<ILogger, string, Exception?> LogInformation =
        LoggerMessage.Define<string>(LogLevel.Information, new EventId(0, nameof(LogInformation)), "{Message}");
}