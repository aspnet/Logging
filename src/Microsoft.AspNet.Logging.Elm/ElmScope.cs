using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Framework.Logging;

namespace Microsoft.Framework.Logging.Elm
{
    public class ElmScope : IDisposable
    {
        private bool _isDisposed;
        private readonly Stopwatch _stopwatch;
        private readonly ILogger _logger;
        private readonly object _state;
        private readonly Guid _request;
        private readonly Guid _id;

        // Maps a request id to a list of Guids representing each scope within that request
        public static IDictionary<Guid, IList<Guid>> Counts = new Dictionary<Guid, IList<Guid>>();
        private readonly object _lock = new object();

        public ElmScope(ILogger logger, object state, Guid request)
        {
            _logger = logger;
            _state = state;
            _request = request;
            _id = Guid.NewGuid();
            _stopwatch = Stopwatch.StartNew();
            lock (_lock)
            {
                if (!Counts.ContainsKey(_request))
                {
                    Counts.Add(_request, new List<Guid>());
                }
                Counts[_request].Add(_id);
            }
            _logger.WriteInformation(string.Format("Begin {0}", _state));
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _stopwatch.Stop();
                _logger.WriteInformation(string.Format("Completed {0} in {1}ms", _state, _stopwatch.ElapsedMilliseconds));
                lock (_lock)
                {
                    Counts[_request].Remove(_id);
                }
                _isDisposed = true;
            }
        }
    }
}