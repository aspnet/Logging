using System;
using System.Collections.Generic;
using NLog;
using Logger = System.Diagnostics.Tracing.Logger;

namespace Microsoft.Extensions.Logging.NLog
{
    public class NLogProvider: IObserver<Logger>, IDisposable
    {
        public NLogProvider(LogFactory nlogFactory)
        {
            _logFactory = nlogFactory;
            _subscriptions = new List<IDisposable>();
        }

        public void Dispose()
        {
            foreach(var subscription in _subscriptions)
            {
                subscription.Dispose();
            }
            _logFactory.Flush();
            _logFactory.Dispose();
        }

        public void OnNext(Logger logger)
        {
            var nlogger = _logFactory.GetLogger(logger.Name);
            var observer = new NLogObserver(nlogger);
            _subscriptions.Add(logger.Subscribe(observer, observer.IsEnabled));
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        private readonly LogFactory _logFactory;
        private List<IDisposable> _subscriptions;
    }
}
