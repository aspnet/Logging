﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Xunit;
#if ASPNET50
using Moq;
#endif

namespace Microsoft.Framework.Logging.Test
{
    public class DiagnosticsLoggerTest
    {
#if ASPNET50
        [Fact]
        public static void IsEnabledReturnsCorrectValue()
        {
            var testSwitch = new SourceSwitch("TestSwitch", "Level will be set to warning for this test");
            testSwitch.Level = SourceLevels.Warning;

            var factory = new LoggerFactory();
            var logger = factory.Create("Test");

            factory.AddProvider(new DiagnosticsLoggerProvider(testSwitch, new ConsoleTraceListener()));

            Assert.True(logger.IsEnabled(TraceType.Critical));
            Assert.True(logger.IsEnabled(TraceType.Error));
            Assert.True(logger.IsEnabled(TraceType.Warning));
            Assert.False(logger.IsEnabled(TraceType.Information));
            Assert.False(logger.IsEnabled(TraceType.Verbose));
        }

        [Theory]
        [InlineData(SourceLevels.Warning, SourceLevels.Information, true)]
        [InlineData(SourceLevels.Information, SourceLevels.Information, true)]
        [InlineData(SourceLevels.Information, SourceLevels.Warning, true)]
        [InlineData(SourceLevels.Warning, SourceLevels.Warning, false)]
        public static void MultipleLoggers_IsEnabledReturnsCorrectValue(SourceLevels first, SourceLevels second, bool expected)
        {
            var firstSwitch = new SourceSwitch("FirstSwitch", "First Test Switch");
            firstSwitch.Level = first;

            var secondSwitch = new SourceSwitch("SecondSwitch", "Second Test Switch");
            secondSwitch.Level = second;

            var factory = new LoggerFactory();
            var logger = factory.Create("Test");

            factory.AddProvider(new DiagnosticsLoggerProvider(firstSwitch, new ConsoleTraceListener()));
            factory.AddProvider(new DiagnosticsLoggerProvider(secondSwitch, new ConsoleTraceListener()));
            Assert.Equal(expected, logger.IsEnabled(TraceType.Information));
        }
    }
#endif
}