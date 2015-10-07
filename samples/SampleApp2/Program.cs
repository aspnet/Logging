using System;
using Microsoft.Framework.Logging;
using System.Diagnostics.Tracing;
using Microsoft.Framework.Logging.Observer;

namespace SampleApp2
{
    public class Program
    {
        private readonly Logger _logger;

        public Program()
        {
            // a DI based application would get ILoggerFactory injected instead
            var factory = new LoggerFactory();

            // getting the logger immediately using the class's name is conventional
            _logger = factory.CreateSystemLogger(typeof(Program).FullName);

            var observer = new ConsoleObserver();

            factory.Subscribe(observer);

        }

        public void Main(string[] args)
        {
            _logger.Log(System.Diagnostics.Tracing.LogLevel.Informational, "InfoMessage", "Starting");

            var startTime = DateTimeOffset.UtcNow;
            _logger.Log(System.Diagnostics.Tracing.LogLevel.Informational, "InfoMessage", new { startTime = startTime, hex = 0x42 });


            try
            {
                throw new Exception("Boom");
            }
            catch (Exception ex)
            {
                _logger.Log(System.Diagnostics.Tracing.LogLevel.Critical, "CriticalMessage", ex);

                // This write should not log anything
                _logger.Log(System.Diagnostics.Tracing.LogLevel.Critical, "CriticalMessage", null);

                _logger.Log(System.Diagnostics.Tracing.LogLevel.Error, "CriticalMessage", ex);
                _logger.Log(System.Diagnostics.Tracing.LogLevel.Warning, "CriticalMessage", ex);
            }


            using (_logger.ActivityStart(System.Diagnostics.Tracing.LogLevel.Informational, "Main", "Main"))
            {
                Console.WriteLine("Hello World");

                _logger.Log(System.Diagnostics.Tracing.LogLevel.Informational, "InfoMessage", "Waiting for user input");
                Console.ReadLine();
            }



            var endTime = DateTimeOffset.UtcNow;
            _logger.Log(System.Diagnostics.Tracing.LogLevel.Informational, "InfoMessage", new { StopTime = endTime });

            _logger.Log(System.Diagnostics.Tracing.LogLevel.Informational, "InfoMessage", "Stopping");

            _logger.Log(System.Diagnostics.Tracing.LogLevel.Informational, "InfoMessage", Environment.NewLine);
            _logger.Log(System.Diagnostics.Tracing.LogLevel.Informational, "InfoMessage", new { Result = "SUCCESS", StartTime = startTime, EndTime = endTime, Duration = (endTime - startTime).TotalMilliseconds });
        }
    }
}
