using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

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

            // Qhen console settings are defined, then only the categories listed are logged;
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
            var logger_A_1 = factory.CreateLogger("CompanyA.Namespace1");
            var logger_A_1_a = factory.CreateLogger("CompanyA.Namespace1.ClassA");
            var logger_A_1_b = factory.CreateLogger("CompanyA.Namespace1.ClassB");
            var logger_A_2 = factory.CreateLogger("CompanyA.Namespace2");
            var logger_A_2_c = factory.CreateLogger("CompanyA.Namespace2.ClassC");
            var logger_B_3_d = factory.CreateLogger("CompanyB.Namespace3.ClassD");

            logger_A_1.LogInformation(1001, "A_1 Information");
            logger_A_1_a.LogInformation(1101, "A_1_a Information");
            logger_A_1_b.LogInformation(1201, "A_1_b Information");
            logger_A_2.LogInformation(1301, "A_2 Information");
            logger_A_2_c.LogInformation(1401, "A_2_c Information");
            logger_B_3_d.LogInformation(1501, "B_3_d Information");

            logger_A_1.LogDebug("A_1 Debug");
            logger_A_1_a.LogDebug("A_1_a Debug");
            logger_A_1_b.LogDebug("A_1_b Debug");
            logger_A_2.LogDebug("A_2 Debug");
            logger_A_2_c.LogDebug("A_2_c Debug");
            logger_B_3_d.LogDebug("B_3_d Debug");

            logger_A_1.LogInformation(1002, "A_1 Information 2");
            logger_A_1_a.LogInformation(1102, "A_1_a Information 2");
            logger_A_1_b.LogInformation(1202, "A_1_b Information 2");
            logger_A_2.LogInformation(1302, "A_2 Information 2");
            logger_A_2_c.LogInformation(1402, "A_2_c Information 2");
            logger_B_3_d.LogInformation(1502, "B_3_d Information 2");

            logger_A_1.LogDebug("A_1 Debug 2");
            logger_A_1_a.LogDebug("A_1_a Debug 2");
            logger_A_1_b.LogDebug("A_1_b Debug 2");
            logger_A_2.LogDebug("A_2 Debug 2");
            logger_A_2_c.LogDebug("A_2_c Debug 2");
            logger_B_3_d.LogDebug("B_3_d Debug 2");

            logger_A_1.LogWarning(4001, "A_1 Warning");
            logger_A_1_a.LogWarning(4101, "A_1_a Warning");
            logger_A_1_b.LogError(5201, "A_1_b Error");
            logger_A_2.LogWarning(4301, "A_2 Warning");
            logger_A_2_c.LogWarning(4401, "A_2_c Warning");
            try
            {
                throw new Exception("Sample");
            }
            catch (Exception ex)
            {
                logger_B_3_d.LogCritical(9501, ex, "B_3_d Critical");
            }
        }
    }
}
