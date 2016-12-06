// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
            var loggerNamespace1 = factory.CreateLogger("CompanyA.Namespace1");
            var loggerClassA = factory.CreateLogger<CompanyA.Namespace1.ClassA>();
            var loggerClassB = factory.CreateLogger<CompanyA.Namespace1.ClassB>();
            var loggerClassC = factory.CreateLogger<CompanyA.Namespace2.ClassC>();
            var loggerNameace3 = factory.CreateLogger("CompanyB.Namespace3");
            var loggerClassD = factory.CreateLogger<CompanyB.Namespace3.ClassD>();

            loggerNamespace1.LogInformation(LoggingEvents.ConfigurationStart, "A_1 Information - namespace level Configuration Start");
            loggerClassA.LogInformation(LoggingEvents.SystemStart, "A_1_a Information - System Start");
            loggerClassB.LogInformation(LoggingEvents.SystemEvent, "A_1_b Information - System Event");
            loggerClassC.LogInformation(LoggingEvents.ConnectionEvent, "A_2_c Information - Connection Event");
            loggerNameace3.LogInformation(2301, "B_3 Information - namespace level");
            loggerClassD.LogInformation(LoggingEvents.AuthenticationSuccess, "B_3_d Information - Authentication Success");

            try
            {
                loggerNamespace1.LogDebug("A_1 Debug");
                loggerClassA.LogDebug("A_1_a Debug - Class A detail");
                loggerClassB.LogDebug("A_1_b Debug - Class B detail");
                loggerClassC.LogDebug("A_2_c Debug");
                loggerNameace3.LogDebug("A_2 Debug");
                loggerClassD.LogDebug("B_3_d Debug");

                loggerClassA.LogDebug("A_1_a Debug 2 - Class A more detail");
                loggerClassB.LogDebug("A_1_b Debug 2 - Class B more detail");
                loggerClassC.LogDebug("A_2_c Debug 2");
                loggerClassD.LogDebug("B_3_d Debug 2");

                loggerClassA.LogDebug("A_1_a Debug 3 - Class A even more detail");
                loggerClassB.LogDebug("A_1_b Debug 3 - Class B even more detail");

                loggerClassA.LogDebug("A_1_a Debug 4 - Class A still going");
                loggerClassB.LogDebug("A_1_b Debug 4 - Class B still going");

                loggerNamespace1.LogWarning(LoggingEvents.ConfigurationWarning, "A_1 Warning - namespace level Configuration Warning");
                loggerClassA.LogWarning(LoggingEvents.SystemWarning, "A_1_a Warning - System Warning");
                loggerClassB.LogError(LoggingEvents.SystemError, "A_1_b Error - System Error");
                loggerClassC.LogWarning(LoggingEvents.ConnectionWarning, "A_2_c Warning - Connection Warning");

                TestExceptionLogging();
            }
            catch (Exception ex)
            {
                loggerClassD.LogCritical(LoggingEvents.AuthenticationCriticalError, ex, "B_3_d Critical = Authentication Critical Error");
            }

            loggerClassC.LogInformation(LoggingEvents.ConnectionStop, "B_3_d Information 2 - Connection Stop");
            loggerClassB.LogInformation(LoggingEvents.SystemStop, "A_1_b Information 2 - System Stop");
            loggerClassA.LogInformation(6102, "A_1_a Information 2");
        }

        private static void TestExceptionLogging()
        {
            throw new NotImplementedException();
        }
    }
}
