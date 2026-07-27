using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Soundboword.Logging;

public static class LoggingBuilderExtensions
{

    extension(ILoggingBuilder builder)
    {

        public ILoggingBuilder AddAvalonia()
        {
            if (LogPreferences.Instance.AppLevel == LogLevel.None)
                return builder;
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, AvaloniaLoggerProvider>());
            return builder;
        }

        public void AddFile()
        {
            if (LogPreferences.Instance.FileLevel != LogLevel.None)
                builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, FileLoggerProvider>());
        }

    }

}
