using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;

namespace Microsoft.Extensions.Logging.Console
{
    public class ConsoleProvider : IObserver<Logger>, IDisposable
    {
        public ConsoleProvider(Func<string, LogLevel, bool> filter)
        {
            _filter = filter;
            _subscriptions = new List<IDisposable>();
        }

        public void Dispose()
        {
            foreach (var subscription in _subscriptions)
            {
                subscription.Dispose();
            }
        }

        public void OnNext(Logger logger)
        {
            var observer = new ConsoleObserver(logger.Name, _filter);
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

        private readonly Func<string, LogLevel, bool> _filter;
        private List<IDisposable> _subscriptions;
    }
}
