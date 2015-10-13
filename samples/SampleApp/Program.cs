using System;
using Microsoft.Extensions.Logging;
using System.Diagnostics.Tracing;
using Microsoft.Extensions.Logging.Observer;

namespace SampleApp
{
    public class Program
    {
        private readonly ILogger _logger;

        public Program()
        {
            // a DI based application would get ILoggerFactory injected instead
            var factory = new LoggerFactory();

            factory.MaximumLevel = LogLevel.Verbose;

            _logger = factory.CreateLogger(typeof(Program).FullName);

            var observer = new ConsoleObserver((name, level) => level < LogLevel.Verbose);

            factory.Subscribe(observer, observer.Filter);

        }

        public void Main(string[] args)
        {
            _logger.Log("InfoMessage", LogLevel.Informational, "Starting");

            var startTime = DateTimeOffset.UtcNow;
            _logger.Log("InfoMessage", LogLevel.Informational, new { startTime = startTime, hex = 0x42 });


            try
            {
                throw new Exception("Boom");
            }
            catch (Exception ex)
            {
                _logger.Log("CriticalMessage", LogLevel.Critical,  ex);

                // This write should not log anything
                _logger.Log("CriticalMessage", LogLevel.Critical, null);

                _logger.Log("CriticalMessage", LogLevel.Error, ex);
                _logger.Log("CriticalMessage", LogLevel.Warning, ex);
            }


            using (_logger.ActivityStart("Main"))
            {
                Console.WriteLine("Hello World");

                _logger.Log("InfoMessage", LogLevel.Informational, "Waiting for user input");
                Console.ReadLine();
            }



            var endTime = DateTimeOffset.UtcNow;
            _logger.Log("InfoMessage", LogLevel.Informational, new { StopTime = endTime });

            _logger.Log("InfoMessage", LogLevel.Informational, "Stopping");

            _logger.Log("InfoMessage", LogLevel.Informational, new { Result = "SUCCESS", StartTime = startTime, EndTime = endTime, Duration = (endTime - startTime).TotalMilliseconds });
        }
    }
}
