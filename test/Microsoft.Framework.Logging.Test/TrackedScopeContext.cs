using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.Logging.Internal;

namespace Microsoft.Framework.Logging.Test
{
    public class TrackedScopeContext : IDisposable
    {
        private readonly ILogger _logger;
        private readonly string _endMessage;
        private readonly Stopwatch _stopwatch;
        private readonly bool _trackTime;
        private readonly LogLevel _logLevel;
        private bool _disposed;

        public TrackedScopeContext(ILogger logger, LogLevel logLevel, string endMessage, bool trackTime)
        {
            _endMessage = endMessage;
            _logger = logger;
            _logLevel = logLevel;
            _trackTime = trackTime;

            if (trackTime)
            {
                _stopwatch = new Stopwatch();
                _stopwatch.Start();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_endMessage != null && _logger.IsEnabled(_logLevel))
                    {
                        _logger.Log(_logLevel, 0, _endMessage, null, null);
                        if (_trackTime)
                        {
                            _logger.Log(_logLevel, 0, $"Elapsed: {_stopwatch.Elapsed}", null, null);
                        }
                    }
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
