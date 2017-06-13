using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging.AzureAppServices
{
    public abstract class BatchingLoggerProvider: ILoggerProvider
    {
        private readonly List<LogMessage> _currentBatch = new List<LogMessage>();
        private readonly TimeSpan _interval;

        private readonly BlockingCollection<LogMessage> _messageQueue;
        private Task _outputTask;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly int? _batchSize;
        private bool _isEnabled;
        private IDisposable _optionsChangeToken;

        protected BatchingLoggerProvider(IOptionsMonitor<BatchingLoggerOptions> options)
        {
            // NOTE: Only IsEnabled is monitored

            var loggerOptions = options.CurrentValue;
            if (loggerOptions.BatchSize < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(loggerOptions.BatchSize), $"{nameof(loggerOptions.BatchSize)} must be a positive number.");
            }
            if (loggerOptions.FlushPeriod <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(loggerOptions.FlushPeriod), $"{nameof(loggerOptions.FlushPeriod)} must be longer than zero.");
            }

            if (loggerOptions.BackgroundQueueSize == null)
            {
                _messageQueue = new BlockingCollection<LogMessage>(new ConcurrentQueue<LogMessage>());
            }
            else
            {
                _messageQueue = new BlockingCollection<LogMessage>(new ConcurrentQueue<LogMessage>(), loggerOptions.BackgroundQueueSize.Value);
            }

            _interval = loggerOptions.FlushPeriod;
            _batchSize = loggerOptions.BatchSize;

            _optionsChangeToken = options.OnChange(UpdateOptions);
            UpdateOptions(options.CurrentValue);
        }

        private void UpdateOptions(BatchingLoggerOptions options)
        {
            var oldIsEnabled = _isEnabled;
            _isEnabled = options.IsEnabled;
            if (oldIsEnabled != _isEnabled)
            {
                if (_isEnabled)
                {
                    Start();
                }
                else
                {
                    Stop();
                }
            }

        }

        protected abstract Task WriteMessagesAsync(IEnumerable<LogMessage> messages);

        private async Task ProcessLogQueue(object state)
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                var limit = _batchSize ?? int.MaxValue;

                while (limit > 0 && _messageQueue.TryTake(out var message))
                {
                    _currentBatch.Add(message);
                    limit--;
                }

                if (_currentBatch.Count > 0)
                {
                    try
                    {
                        await WriteMessagesAsync(_currentBatch);
                    }
                    catch
                    {
                        // ignored
                    }

                    _currentBatch.Clear();
                }

                await IntervalAsync(_interval, _cancellationTokenSource.Token);
            }
        }

        protected virtual Task IntervalAsync(TimeSpan interval, CancellationToken cancellationToken)
        {
            return Task.Delay(interval, cancellationToken);
        }

        internal void AddMessage(DateTimeOffset timestamp, string message)
        {
            if (!_messageQueue.IsAddingCompleted)
            {
                try
                {
                    _messageQueue.Add(new LogMessage { Message = message, Timestamp = timestamp }, _cancellationTokenSource.Token);
                }
                catch
                {
                    //cancellation token canceled or CompleteAdding called
                }
            }
        }

        private void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _outputTask = Task.Factory.StartNew(
                ProcessLogQueue,
                null,
                TaskCreationOptions.LongRunning);
        }

        private void Stop()
        {
            _cancellationTokenSource.Cancel();
            _messageQueue.CompleteAdding();

            try
            {
                _outputTask.Wait(_interval);
            }
            catch (TaskCanceledException)
            {
            }
            catch (AggregateException ex) when (ex.InnerExceptions.Count == 1 && ex.InnerExceptions[0] is TaskCanceledException)
            {
            }
        }

        public void Dispose()
        {
            _optionsChangeToken?.Dispose();
            if (_isEnabled)
            {
                Stop();
            }
        }

        public ILogger CreateLogger(string categoryName)
        {
            if (!_isEnabled)
            {
                return NullLogger.Instance;
            }

            return new BatchingLogger(this, categoryName);
        }

    }

    public struct LogMessage
    {
        public DateTimeOffset Timestamp { get; set; }
        public string Message { get; set; }
    }

    public class BatchingLogger : ILogger
    {
        private readonly BatchingLoggerProvider _provider;
        private readonly string _category;

        public BatchingLogger(BatchingLoggerProvider loggerProvider, string categoryName)
        {
            this._provider = loggerProvider;
            this._category = categoryName;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(DateTimeOffset timestamp, LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var builder = new StringBuilder();
            builder.Append(timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff zzz"));
            builder.Append(" [");
            builder.Append(logLevel.ToString());
            builder.Append("] ");
            builder.Append(_category);
            builder.Append(": ");
            builder.AppendLine(formatter(state, exception));

            _provider.AddMessage(timestamp, builder.ToString());
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Log(DateTimeOffset.Now, logLevel, eventId, state, exception, formatter);
        }
    }
}
