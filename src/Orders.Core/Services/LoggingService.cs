using Microsoft.Extensions.Logging;
using Orders.Core.Interfaces;

namespace Orders.Core.Services;

public class LoggingService : ILoggingService
{
    public async Task LogToFile(LogLevel logLevel, string logMessage, Exception? exception = null)
    {
        try
        {
            // Determine the log file name based on the log level
            var logFileName = logLevel switch
            {
                LogLevel.Information => "info.log",
                LogLevel.Warning => "warning.log",
                LogLevel.Error => "error.log",
                LogLevel.Critical => "critical.log",
                _ => "unknown.log"
            };

            var logFilePath = Path.Combine("Logs", logFileName);

            // Ensure the directory exists before logging
            var logDirectory = Path.GetDirectoryName(logFilePath);
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory!);
            }

            // Create a log entry with various information
            var logEntry = $"{DateTime.Now} [{logLevel}] - {logMessage}";

            // Include exception information if available
            if (exception != null)
            {
                logEntry +=
                    $"\nException: {exception.GetType().Name} - {exception.Message}\nStackTrace: {exception.StackTrace}";
            }

            // Append the log message to the file
            await File.AppendAllTextAsync(logFilePath, logEntry + "\n");
        }
        catch (Exception ex)
        {
            Console.Write("Logging error: " + ex.Message);
        }
    }
}