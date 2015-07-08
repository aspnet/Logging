﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Framework.Logging.TraceSource;
using Xunit;

namespace Microsoft.Framework.Logging.Test
{
    public class TraceSourceLoggerProviderTest
    {
        [Theory]
        [InlineData(true, 3)]
        [InlineData(false, 1)]
        public static void Dispose_TraceListenerIsFlushed(bool autoFlush, int expectedFlush)
        {
            // Arrange
            Trace.AutoFlush = autoFlush;
            var testSwitch = new SourceSwitch("TestSwitch", "Level will be set to warning for this test");
            testSwitch.Level = SourceLevels.Warning;
            var listener = new BufferedConsoleTraceListener();

            TraceSourceLoggerProvider provider = new TraceSourceLoggerProvider(testSwitch, listener);
            var logger1 = provider.CreateLogger("FirstLogger");
            var logger2 = provider.CreateLogger("SecondLogger");
            logger1.LogError("message1");
            logger2.LogError("message2");
            
            // Act
            provider.Dispose();

            // Assert
            Assert.Equal(expectedFlush, listener.FlushCount);
        }

        private class BufferedConsoleTraceListener : ConsoleTraceListener
        {
            public int FlushCount { get; set; }

            public override void Flush()
            {
                FlushCount++;
                base.Flush();
            }
        }
    }
}