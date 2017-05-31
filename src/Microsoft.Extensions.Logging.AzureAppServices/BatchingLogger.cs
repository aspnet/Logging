using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Logging.AzureAppServices
{
    public class FileLoggerProvider : BatchingLoggerProvider
    {
        private readonly string path;
        private readonly string _fileName;
        private readonly int? maxFileSize;
        private readonly int? maxRetainedFiles;

        public FileLoggerProvider(
            string path,
            string fileName,
            int? maxFileSize,
            int? maxRetainedFiles,
            TimeSpan interval,
            int? maxBatchSize,
            int? maxQueueSize) : base(interval, maxBatchSize, maxQueueSize)
        {
            this.path = path;
            _fileName = fileName;
            this.maxFileSize = maxFileSize;
            this.maxRetainedFiles = maxRetainedFiles;
        }

        protected override async Task WriteMessagesAsync(IEnumerable<LogMessage> messages)
        {
            Directory.CreateDirectory(path);

            foreach (var group in messages.GroupBy(GetGrouping))
            {
                var fullName = GetFullName(group.Key);
                var fileInfo = new FileInfo(fullName);
                if (maxFileSize > 0 && fileInfo.Exists && fileInfo.Length > maxFileSize)
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
            return Path.Combine(path, $"{_fileName}.{group.Year:0000}{group.Month:00}{group.Day:00}.log");
        }
        
        public (int Year, int Month, int Day) GetGrouping(LogMessage message)
        {
            return (message.TimeSpan.Year, message.TimeSpan.Month, message.TimeSpan.Day);
        }

        protected void RollFiles()
        {
            if (maxRetainedFiles > 0)
            {
                var files = new DirectoryInfo(path)
                    .GetFiles(_fileName + "*")
                    .OrderByDescending(f => f.CreationTime)
                    .Skip(maxRetainedFiles.Value);

                foreach (var item in files)
                {
                    item.Delete();
                }
            }
        }
    }

    public abstract class BatchingLoggerProvider: IDisposable, ILoggerProvider
    {
        private readonly List<LogMessage> _currentBatch = new List<LogMessage>();
        private readonly TimeSpan _interval;

        private readonly BlockingCollection<LogMessage> _messageQueue;
        private readonly Task _outputTask;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly int? _batchSize;

        protected BatchingLoggerProvider(TimeSpan interval, int? maxBatchSize, int? maxQueueSize)
        {
            if (maxQueueSize == null)
            {
                _messageQueue = new BlockingCollection<LogMessage>(new ConcurrentQueue<LogMessage>());
            }
            else
            {
                _messageQueue = new BlockingCollection<LogMessage>(new ConcurrentQueue<LogMessage>(), maxQueueSize.Value);
            }

            _interval = interval;
            _batchSize = maxBatchSize;
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
                    _messageQueue.Add(new LogMessage() { Message = message, TimeSpan = timestamp }, _cancellationTokenSource.Token);
                }
                catch
                {
                    //cancellation token cancelled or CompleteAdding called
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
        public DateTimeOffset TimeSpan { get; set; }
        public string Message { get; set; }
    }

    public class BatchingLogger : ILogger
    {
        private BatchingLoggerProvider _provider;
        private string _category;

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
