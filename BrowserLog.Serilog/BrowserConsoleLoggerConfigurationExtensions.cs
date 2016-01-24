using System;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting.Display;

namespace BrowserLog.Serilog
{
    public static class BrowserConsoleLoggerConfigurationExtensions
    {
        public const string DefaultOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}";
        /// <summary>
        /// Use Server Side Event to broadcast logs to a debugging browser console.
        /// </summary>
        /// <param name="sinkConfiguration">Logger sink configuration.</param>
        /// <param name="restrictedToMinimumLevel">The minimum level for
        /// events passed through the sink. Ignored when <paramref name="levelSwitch"/> is specified.</param>
        /// <param name="active">Activate or not the logging (default: false)</param>
        /// <param name="port">Port on which the browser should connect (default: 8082)</param>
        /// <param name="buffer">Size of the buffer (default: 100)</param>
        /// <param name="outputTemplate">The template used to format the message (default: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")</param>
        /// <param name="formatProvider">Format provider that should be used when formating the message (default: null)</param>
        /// <param name="logProperties">Enable logging of meta-properties (default: false)</param>
        /// <returns>Configuration object allowing method chaining.</returns>
        public static LoggerConfiguration BrowserConsole(
            this LoggerSinkConfiguration sinkConfiguration,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            bool active = false,
            int port = 8082,
            int buffer = 100,
            string outputTemplate = DefaultOutputTemplate,
            IFormatProvider formatProvider = null,
            bool logProperties = false)
        {
            var formatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
            return sinkConfiguration.Sink(new BrowserConsoleSink(active, port, buffer, formatter, logProperties), restrictedToMinimumLevel);
        }
    }
}
