// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;

namespace Microsoft.Extensions.Logging
{
    public class LogSinkProvider : ILogSinkProvider
    {
        private ILogSink[] _sinks = new ILogSink[0];
        private readonly object _sync = new object();
        private bool _disposed;

        public ILogSink[] Sinks => _sinks;

        public ILogFilter Filter { get; set; }

        public void AddSink(ILogSink sink)
        {
            lock (_sync)
            {
                _sinks = _sinks.Concat(new[] { sink }).ToArray();
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                foreach (var sink in _sinks)
                {
                    try
                    {
                        sink.Dispose();
                    }
                    catch
                    {
                        // Swallow exceptions on dispose
                    }
                }

                _disposed = true;
            }
        }
    }
}
