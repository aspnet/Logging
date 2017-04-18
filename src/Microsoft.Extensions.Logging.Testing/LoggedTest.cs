using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Xunit.Abstractions;

namespace Microsoft.Extensions.Logging.Testing
{
    public abstract class LoggedTest
    {
        private readonly ITestOutputHelper _output;

        public LoggedTest(ITestOutputHelper output)
        {
            _output = output;
        }

        public IDisposable StartLog(out ILoggerFactory loggerFactory, [CallerMemberName] string testName = null)
        {
            return AssemblyTestLog.ForAssembly(GetType().GetTypeInfo().Assembly).StartTestLog(_output, GetType().FullName, out loggerFactory, testName);
        }
    }
}
