// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging.Analyzers;
using Xunit;

namespace Microsoft.Extensions.Logging.Analyzer.Test
{
    public class UseCompiledLogMessagesAnalyzerTests : DiagnosticVerifier
    {
        [Theory]
        [InlineData("LogTrace", @"""This is a test {Message}"", ""Foo""")]
        [InlineData("LogDebug", @"""This is a test {Message}"", ""Foo""")]
        [InlineData("LogInformation", @"""This is a test {Message}"", ""Foo""")]
        [InlineData("LogWarning", @"""This is a test {Message}"", ""Foo""")]
        [InlineData("LogError", @"""This is a test {Message}"", ""Foo""")]
        [InlineData("LogCritical", @"""This is a test {Message}"", ""Foo""")]
        [InlineData("BeginScope", @"""This is a test {Message}"", ""Foo""")]
        public void DiagnosticIsProducedForInvocationsOfAllLoggerExtensions(string method, string args)
        {
            var diagnostic = Assert.Single(GetDiagnostics(method, args));
            Assert.Equal("MEL0004", diagnostic.Id);
            Assert.Equal($"For improved performance, use pre-compiled log messages instead of calling '{method}' with a string message.", diagnostic.GetMessage());
        }

        [Theory]
        [InlineData("Log", "LogLevel.Debug, 1, state: new object(), exception: new System.Exception(), formatter: null")]
        [InlineData("BeginScope", "new object()")]
        [InlineData("IsEnabled", "LogLevel.Debug")]
        public void DiagnosticIsNotProducedForMethodsOnILogger(string method, string args)
        {
            Assert.Empty(GetDiagnostics(method, args));
        }

        private static Diagnostic[] GetDiagnostics(string method, string args)
        {
            var code = $@"
using Microsoft.Extensions.Logging;
public class Program
{{
    public const string Const = ""const"";
    public static void Main()
    {{
        ILogger logger = null;
        logger.{method}({args});
    }}
}}
";
            return GetSortedDiagnostics(new[] { code }, new UseCompiledLogMessagesAnalyzer());
        }
    }
}
