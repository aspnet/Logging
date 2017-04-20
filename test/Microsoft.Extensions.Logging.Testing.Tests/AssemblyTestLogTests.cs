﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.Extensions.Logging.Testing.Tests
{
    public class AssemblyTestLogTests : LoggedTest
    {
        private static readonly Assembly ThisAssembly = typeof(AssemblyTestLog).GetTypeInfo().Assembly;

        public AssemblyTestLogTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void ForAssembly_ReturnsSameInstanceForSameAssembly()
        {
            Assert.Same(
                AssemblyTestLog.ForAssembly(ThisAssembly),
                AssemblyTestLog.ForAssembly(ThisAssembly));
        }

        [Fact]
        public void TestLogWritesToITestOutputHelper()
        {
            var output = new TestTestOutputHelper();
            var assemblyLog = AssemblyTestLog.Create("NonExistant.Test.Assembly", baseDirectory: null);

            using (assemblyLog.StartTestLog(output, "NonExistant.Test.Class", out var loggerFactory))
            {
                var logger = loggerFactory.CreateLogger("TestLogger");
                logger.LogInformation("Information!");
            }

            Assert.Equal(@"| TestLifetime Information: Starting test TestLogWritesToITestOutputHelper
| TestLogger Information: Information!
| TestLifetime Information: Finished test TestLogWritesToITestOutputHelper in DURATION
", MakeConsistent(output.Output));
        }

        [Fact]
        public void TestLogWritesToGlobalLogFile()
        {
            // Because this test writes to a file, it is a functional test and should be logged
            // but it's also testing the test logging facility. So this is pretty meta ;)
            using (StartLog(out var loggerFactory))
            {
                var logger = loggerFactory.CreateLogger("Test");

                var tempDir = Path.Combine(Path.GetTempPath(), $"TestLogging_{Guid.NewGuid().ToString("N")}");
                using (var testAssemblyLog = AssemblyTestLog.Create("FakeTestAssembly", tempDir))
                {
                    logger.LogInformation("Created test log in {baseDirectory}", tempDir);

                    using (testAssemblyLog.StartTestLog(output: null, className: "FakeTestAssembly.FakeTestClass", loggerFactory: out var testLoggerFactory, testName: "FakeTestName"))
                    {
                        var testLogger = testLoggerFactory.CreateLogger("TestLogger");
                        testLogger.LogInformation("Information!");
                    }
                }

                logger.LogInformation("Finished test log in {baseDirectory}", tempDir);

                var globalLogPath = Path.Combine(tempDir, "FakeTestAssembly", "global.log");
                var testLog = Path.Combine(tempDir, "FakeTestAssembly", "FakeTestClass", $"FakeTestName.log");

                Assert.True(File.Exists(globalLogPath), $"Expected global log file {globalLogPath} to exist");
                Assert.True(File.Exists(testLog), $"Expected test log file {testLog} to exist");

                var globalLogContent = MakeConsistent(File.ReadAllText(globalLogPath));
                logger.LogInformation($"Global Log Content:{Environment.NewLine}{{content}}", globalLogContent);
                var testLogContent = MakeConsistent(File.ReadAllText(testLog));
                logger.LogInformation($"Test Log Content:{Environment.NewLine}{{content}}", testLogContent);

                Assert.Equal(@"[GlobalTestLog] [Information] Global Test Logging initialized. Set the 'ASPNETCORE_TEST_LOG_DIR' Environment Variable in order to create log files on disk.
[GlobalTestLog] [Information] Starting test ""FakeTestName""
[GlobalTestLog] [Information] Finished test ""FakeTestName"" in DURATION
", globalLogContent);
                Assert.Equal(@"[TestLifetime] [Information] Starting test ""FakeTestName""
[TestLogger] [Information] Information!
[TestLifetime] [Information] Finished test ""FakeTestName"" in DURATION
", testLogContent);
            }
        }

        private static readonly Regex DurationRegex = new Regex(@"[^ ]+s$");
        private static string MakeConsistent(string input)
        {
            return string.Join(Environment.NewLine, input.Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                .Select(line => DurationRegex.Replace(line.IndexOf("[") >= 0 ? line.Substring(line.IndexOf("[")) : line, "DURATION")));
        }
    }
}
