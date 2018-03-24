using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.Extensions.Logging.Internal;

namespace Microsoft.Extensions.Logging.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseCompiledLogMessagesAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptors.MEL4UseCompiledLogMessages);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(analysisContext =>
            {
                var loggerExtensionsType = analysisContext.Compilation.GetTypeByMetadataName("Microsoft.Extensions.Logging.LoggerExtensions");
                if (loggerExtensionsType == null)
                {
                    return;
                }

                analysisContext.RegisterSyntaxNodeAction(syntaxContext => AnalyzeInvocation(syntaxContext, loggerExtensionsType), SyntaxKind.InvocationExpression);
            });
        }

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext syntaxContext, INamedTypeSymbol loggerExtensionsType)
        {
            var invocation = (InvocationExpressionSyntax)syntaxContext.Node;

            var symbolInfo = ModelExtensions.GetSymbolInfo(syntaxContext.SemanticModel, invocation, syntaxContext.CancellationToken);
            if (symbolInfo.Symbol?.Kind != SymbolKind.Method)
            {
                return;
            }

            var methodSymbol = (IMethodSymbol)symbolInfo.Symbol;

            if (methodSymbol.ContainingType != loggerExtensionsType)
            {
                return;
            }

            syntaxContext.ReportDiagnostic(Diagnostic.Create(Descriptors.MEL4UseCompiledLogMessages, invocation.GetLocation(), methodSymbol.Name));
        }
    }
}
