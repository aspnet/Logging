using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging.AzureAppServices
{
    public class FileLoggerProvider : BatchingLoggerProvider
    {
        private readonly string _path;
        private readonly string _fileName;
        private readonly int? _maxFileSize;
        private readonly int? _maxRetainedFiles;

        public FileLoggerProvider(IOptions<AzureDiagnosticsFileLoggerOptions> options) : base(options)
        {
            var loggerOptions = options.Value;
            _path = loggerOptions.LogDirectory;
            _fileName = loggerOptions.FileName;
            _maxFileSize = loggerOptions.FileSizeLimit;
            _maxRetainedFiles = loggerOptions.RetainedFileCountLimit;
        }

        protected override async Task WriteMessagesAsync(IEnumerable<LogMessage> messages)
        {
            Directory.CreateDirectory(_path);

            foreach (var group in messages.GroupBy(GetGrouping))
            {
                var fullName = GetFullName(group.Key);
                var fileInfo = new FileInfo(fullName);
                if (_maxFileSize > 0 && fileInfo.Exists && fileInfo.Length > _maxFileSize)
                {
                    return;
                }

                using (var streamWriter = File.AppendText(fullName))
                {
                    foreach (var item in group)
                    {
                        await streamWriter.WriteAsync(item.Message);
                    }
                }
            }

            RollFiles();
        }

        private string GetFullName((int Year, int Month, int Day) group)
        {
            return Path.Combine(_path, $"{_fileName}.{group.Year:0000}{group.Month:00}{group.Day:00}.txt");
        }

        public (int Year, int Month, int Day) GetGrouping(LogMessage message)
        {
            return (message.Timestamp.Year, message.Timestamp.Month, message.Timestamp.Day);
        }

        protected void RollFiles()
        {
            if (_maxRetainedFiles > 0)
            {
                var files = new DirectoryInfo(_path)
                    .GetFiles(_fileName + "*")
                    .OrderByDescending(f => f.CreationTime)
                    .Skip(_maxRetainedFiles.Value);

                foreach (var item in files)
                {
                    item.Delete();
                }
            }
        }
    }

    public abstract class BatchingLoggerProvider: ILoggerProvider
    {
        private readonly List<LogMessage> _currentBatch = new List<LogMessage>();
        private readonly TimeSpan _interval;

        private readonly BlockingCollection<LogMessage> _messageQueue;
        private readonly Task _outputTask;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly int? _batchSize;

        protected BatchingLoggerProvider(IOptions<BatchingLoggerOptions> options)
        {
            var loggerOptions = options.Value;
            if (loggerOptions.BatchSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(loggerOptions.BatchSize), $"{nameof(loggerOptions.BatchSize)} must be a positive number.");
            }
            if (loggerOptions.FlushPeriod <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(loggerOptions.FlushPeriod), $"{nameof(loggerOptions.FlushPeriod)} must be longer than zero.");
            }

            if (loggerOptions.BackgroundQueueSize == 0)
            {
                _messageQueue = new BlockingCollection<LogMessage>(new ConcurrentQueue<LogMessage>());
            }
            else
            {
                _messageQueue = new BlockingCollection<LogMessage>(new ConcurrentQueue<LogMessage>(), loggerOptions.BackgroundQueueSize);
            }

            _interval = loggerOptions.FlushPeriod;
            _batchSize = loggerOptions.BatchSize;
            _cancellationTokenSource = new CancellationTokenSource();
            _outputTask = Task.Factory.StartNew(
                ProcessLogQueue,
                null,
                TaskCreationOptions.LongRunning);
        }

        protected abstract Task WriteMessagesAsync(IEnumerable<LogMessage> messages);

        private async Task ProcessLogQueue(object state)
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                var limit = _batchSize == 0 ? int.MaxValue : _batchSize;

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
                    _messageQueue.Add(new LogMessage() { Message = message, Timestamp = timestamp }, _cancellationTokenSource.Token);
                }
                catch
                {
                    //cancellation token canceled or CompleteAdding called
                }
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _messageQueue.CompleteAdding();

            try
            {
                _outputTask.Wait(_interval);
            }
            catch (TaskCanceledException) { }
            catch (AggregateException ex) when (ex.InnerExceptions.Count == 1 && ex.InnerExceptions[0] is TaskCanceledException) { }
        }

        public ILogger CreateLogger(string categoryName)
        {
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
