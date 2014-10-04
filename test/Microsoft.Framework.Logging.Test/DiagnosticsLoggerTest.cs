// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Xunit;
#if ASPNET50
using Moq;
#endif

namespace Microsoft.Framework.Logging.Test
{
    /// <summary>
    /// Summary description for DiagnosticsLoggerTest
    /// </summary>
    public class DiagnosticsLoggerTest
    {
#if ASPNET50
        [Fact]
        public static void DiagnosticsLoggerIsEnabledReturnsCorrectValue()
        {
            SourceSwitch testSwitch = new SourceSwitch("TestSwitch", "Level will be set to warning for this test");
            testSwitch.Level = SourceLevels.Warning;

            var factory = new LoggerFactory();
            ILogger logger = factory.Create("Test");

            factory.AddProvider(new DiagnosticsLoggerProvider(testSwitch, new ConsoleTraceListener()));

            Assert.Equal(true, logger.IsEnabled(TraceType.Critical));
            Assert.Equal(true, logger.IsEnabled(TraceType.Error));
            Assert.Equal(true, logger.IsEnabled(TraceType.Warning));
            Assert.Equal(false, logger.IsEnabled(TraceType.Information));
            Assert.Equal(false, logger.IsEnabled(TraceType.Verbose));
        }
    }
#endif
}