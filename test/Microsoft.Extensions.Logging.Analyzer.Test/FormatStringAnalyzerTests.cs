// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.Logging.Analyzers;
using Xunit;

namespace Microsoft.Extensions.Logging.Analyzer.Test
{
    public class FormatStringAnalyzerTests: DiagnosticVerifier
    {
        [Theory]
        [InlineData(@"logger.LogError(""{0}"", 1);", "MEL1")]
        [InlineData(@"logger.LogError($""{string.Empty}"");", "MEL2")]
        [InlineData(@"logger.LogError(""string"" + 2);", "MEL2")]
        [InlineData(@"logger.LogError(""{string}"");", "MEL3")]
        [InlineData(@"logger.LogError(message: ""{string}"");", "MEL3")]
        [InlineData(@"logger.LogError(""{string}"", 1, 2);", "MEL3")]
        [InlineData(@"logger.LogError(""{str"" + ""ing}"", 1, 2);", "MEL3")]
        [InlineData(@"logger.LogError(""{"" + nameof(ILogger) + ""}"");", "MEL3")]
        [InlineData(@"logger.LogError(""{"" + Const + ""}"");", "MEL3")]

        // Concat would be optimized by compiler
        [InlineData(@"logger.LogError(nameof(ILogger) + "" string"");", null)]
        [InlineData(@"logger.LogError("" string"" + "" string"");", null)]
        [InlineData(@"logger.LogError($"" string"" + $"" string"");", null)]
        [InlineData(@"logger.LogError(""{st"" + ""ring}"", 1);", null)]

        // we are unable to parse expressions
        [InlineData(@"logger.LogError(""{string} {string}"", new object [] {1});", null)]
        public void ProducesCorrectDiagnostics(string expression, string exprectedDiagnostics)
        {
            var code = $@"
using Microsoft.Extensions.Logging;
public class Program
{{
    public const string Const = ""const"";
    public static void Main()
    {{
        ILogger logger = null;
        {expression}
    }}
}}
";
            var diagnostics = GetSortedDiagnostics(new[] { code }, new LogFormatAnalyzer());
            if (exprectedDiagnostics != null)
            {
                var diagnostic = Assert.Single(diagnostics);
                Assert.Equal(exprectedDiagnostics, diagnostic.Id);
            }
            else
            {
                Assert.Empty(diagnostics);
            }
        }
    }
}
