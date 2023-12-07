using Microsoft.Extensions.Logging;

namespace Orders.Core.Interfaces;

public interface ILoggingService
{
    Task LogToFile(LogLevel logLevel, string logMessage, Exception? exception = null);
}   