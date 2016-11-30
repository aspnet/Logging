using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Collections.Generic;

namespace ConsoleHierarchySample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // This example shows how the category hierarchy works for the console logger.

            // Normally loggers are created through the strongly typed CreateLogger<T>, 
            // which means the hierarchy of namespaces can be used to selectively turn
            // sections of logging on and off.

            var factory = new LoggerFactory();

            // When console settings are defined, then only the categories listed are logged;
            // other categories default to the value of the special 'Default' (case sensitive) 
            // category, or if not specified then they are not logged at all.
            var consoleSettings = new ConsoleLoggerSettings();
            consoleSettings.Switches = new Dictionary<string, LogLevel>() {
                { "Default", LogLevel.Warning },
                { "CompanyA.Namespace1.ClassB", LogLevel.Debug },
                { "CompanyB", LogLevel.Information },
            };
            factory.AddConsole(consoleSettings);

            // Normally these are created by an injected ILogger<T> and automatically extract the full class name.
            // Loggers at other levels (e.g. namespace) would need to be created manually.
            var logger_A_1 = factory.CreateLogger("CompanyA.Namespace1");
            var logger_A_1_a = factory.CreateLogger("CompanyA.Namespace1.ClassA");
            var logger_A_1_b = factory.CreateLogger("CompanyA.Namespace1.ClassB");
            var logger_A_2_c = factory.CreateLogger("CompanyA.Namespace2.ClassC");
            var logger_B_3 = factory.CreateLogger("CompanyB.Namespace3");
            var logger_B_3_d = factory.CreateLogger("CompanyB.Namespace3.ClassD");

            logger_A_1.LogInformation(LoggingEvents.ConfigurationStart, "A_1 Information - namespace level Configuration Start");
            logger_A_1_a.LogInformation(LoggingEvents.SystemStart, "A_1_a Information - System Start");
            logger_A_1_b.LogInformation(LoggingEvents.SystemEvent, "A_1_b Information - System Event");
            logger_A_2_c.LogInformation(LoggingEvents.ConnectionEvent, "A_2_c Information - Connection Event");
            logger_B_3.LogInformation(2301, "B_3 Information - namespace level");
            logger_B_3_d.LogInformation(LoggingEvents.AuthenticationSuccess, "B_3_d Information - Authentication Success");

            try
            {
                logger_A_1.LogDebug("A_1 Debug");
                logger_A_1_a.LogDebug("A_1_a Debug - Class A detail");
                logger_A_1_b.LogDebug("A_1_b Debug - Class B detail");
                logger_A_2_c.LogDebug("A_2_c Debug");
                logger_B_3.LogDebug("A_2 Debug");
                logger_B_3_d.LogDebug("B_3_d Debug");

                logger_A_1_a.LogDebug("A_1_a Debug 2 - Class A more detail");
                logger_A_1_b.LogDebug("A_1_b Debug 2 - Class B more detail");
                logger_A_2_c.LogDebug("A_2_c Debug 2");
                logger_B_3_d.LogDebug("B_3_d Debug 2");

                logger_A_1_a.LogDebug("A_1_a Debug 3 - Class A even more detail");
                logger_A_1_b.LogDebug("A_1_b Debug 3 - Class B even more detail");

                logger_A_1_a.LogDebug("A_1_a Debug 4 - Class A still going");
                logger_A_1_b.LogDebug("A_1_b Debug 4 - Class B still going");

                logger_A_1.LogWarning(LoggingEvents.ConfigurationWarning, "A_1 Warning - namespace level Configuration Warning");
                logger_A_1_a.LogWarning(LoggingEvents.SystemWarning, "A_1_a Warning - System Warning");
                logger_A_1_b.LogError(LoggingEvents.SystemError, "A_1_b Error - System Error");
                logger_A_2_c.LogWarning(LoggingEvents.ConnectionWarning, "A_2_c Warning - Connection Warning");

                TestExceptionLogging();
            }
            catch (Exception ex)
            {
                logger_B_3_d.LogCritical(LoggingEvents.AuthenticationCriticalError, ex, "B_3_d Critical = Authentication Critical Error");
            }

            logger_A_2_c.LogInformation(LoggingEvents.ConnectionStop, "B_3_d Information 2 - Connection Stop");
            logger_A_1_b.LogInformation(LoggingEvents.SystemStop, "A_1_b Information 2 - System Stop");
            logger_A_1_a.LogInformation(6102, "A_1_a Information 2");
        }

        private static void TestExceptionLogging()
        {
            throw new NotImplementedException();
        }
    }
}
