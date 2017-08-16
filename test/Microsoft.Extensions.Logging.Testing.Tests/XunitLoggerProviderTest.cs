﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Test;
using Xunit;

namespace Microsoft.Extensions.Logging.Testing.Tests
{
    public class XunitLoggerProviderTest
    {
        [Fact]
        public void LoggerProviderWritesToTestOutputHelper()
        {
            var testTestOutputHelper = new TestTestOutputHelper();

            var loggerFactory = TestLoggerBuilder.Create(builder => builder
                .SetMinimumLevel(LogLevel.Trace)
                .AddXunit(testTestOutputHelper));

            var logger = loggerFactory.CreateLogger("TestCategory");
            logger.LogInformation("This is some great information");
            logger.LogTrace("This is some unimportant information");

            var expectedOutput =
                "| TestCategory Information: This is some great information" + Environment.NewLine +
                "| TestCategory Trace: This is some unimportant information" + Environment.NewLine;

            Assert.Equal(expectedOutput, testTestOutputHelper.Output);
        }

        [Fact]
        public void LoggerProviderDoesNotWriteLogMessagesBelowMinimumLevel()
        {
            var testTestOutputHelper = new TestTestOutputHelper();
            var loggerFactory = TestLoggerBuilder.Create(builder => builder
                .AddXunit(testTestOutputHelper, LogLevel.Warning));

            var logger = loggerFactory.CreateLogger("TestCategory");
            logger.LogInformation("This is some great information");
            logger.LogError("This is a bad error");

            Assert.Equal("| TestCategory Error: This is a bad error" + Environment.NewLine, testTestOutputHelper.Output);
        }

        [Fact]
        public void LoggerProviderPrependsPrefixToEachLine()
        {
            var testTestOutputHelper = new TestTestOutputHelper();
            var loggerFactory = TestLoggerBuilder.Create(builder => builder
                .AddXunit(testTestOutputHelper));

            var logger = loggerFactory.CreateLogger("TestCategory");
            logger.LogInformation("This is a" + Environment.NewLine + "multi-line" + Environment.NewLine + "message");

            var expectedOutput =
                "| TestCategory Information: This is a" + Environment.NewLine +
                "|                           multi-line" + Environment.NewLine +
                "|                           message" + Environment.NewLine;

            Assert.Equal(expectedOutput, testTestOutputHelper.Output);
        }

        [Fact]
        public void LoggerProviderDoesNotThrowIfOutputHelperThrows()
        {
            var testTestOutputHelper = new TestTestOutputHelper();
            var loggerFactory = TestLoggerBuilder.Create(builder => builder
                .AddXunit(testTestOutputHelper));

            testTestOutputHelper.Throw = true;

            var logger = loggerFactory.CreateLogger("TestCategory");
            logger.LogInformation("This is a" + Environment.NewLine + "multi-line" + Environment.NewLine + "message");

            Assert.Equal(0, testTestOutputHelper.Output.Length);
        }
    }
}
