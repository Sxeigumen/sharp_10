using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace lab10New
{
    public class ConsoleLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new ConsoleLogger();
        }
        // если ваш логер использует неуправляемые ресурсы,
        // то вы можете освободить память здесь
        public void Dispose() { }
    }

    public class ConsoleLogger : ILogger
    {
        // если ваш логер использует неуправляемые ресурсы,
        // то здесь вы можете вернуть класс, реализующий IDisposable
        public IDisposable BeginScope<TState>(TState tstate)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            // чтобы избежать ведения лишних логов,
            // можно отфильтровать по уровню логирования
            switch (logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Information:
                case LogLevel.None:
                    return false;
                case LogLevel.Debug:
                case LogLevel.Warning:
                case LogLevel.Error:
                case LogLevel.Critical:
                default:
                    return true;
            };
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (eventId.Id != 20100)
            {
                return;
            }
            // регистрация идентификатора уровня и события
            Console.Write($"Level: {logLevel}, EventID: {eventId.Id}");
            if (state != null)
            {
                // вывод только состояния или исключения при наличии
                Console.Write($", State: {state}");
            }
            if (exception != null)
            {
                Console.Write($", Exception: {exception.Message}");
            }
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}