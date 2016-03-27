// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Extensions.Logging
{
    public class LoggerProviderProvider : ILogSinkProvider
    {
        private ILogSink[] _sinks = new ILogSink[0];
        private readonly object _sync = new object();
        private bool _disposed;

        public ILogSink[] Sinks => _sinks;

        public void AddSink(ILogSink provider)
        {
            lock (_sync)
            {
                _sinks = _sinks.Concat(new[] { provider }).ToArray();
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                foreach (var provider in _sinks)
                {
                    try
                    {
                        provider.Dispose();
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
