using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.Extensions.Logging.AzureAppServices.Test
{

    public class FileLoggerTests: IDisposable
    {
        DateTimeOffset _timestampOne = new DateTimeOffset(2016, 05, 04, 03, 02, 01, TimeSpan.Zero);

        public FileLoggerTests()
        {
            TempPath = Path.GetTempFileName() + "_";
        }

        public string TempPath { get; }

        public void Dispose()
        {
            try
            {
                if (Directory.Exists(TempPath))
                {
                    Directory.Delete(TempPath, true);
                }
            }
            catch
            {
            }
        }

        [Fact]
        public async Task WritesToTextFile()
        {
            var provider = new TestFileLoggerProvider(TempPath);
            var logger = (BatchingLogger)provider.CreateLogger("Cat");

            await provider.IntervalControl.Pause;

            logger.Log(_timestampOne, LogLevel.Information, 0, "Info message", null, (state, ex) => state);
            logger.Log(_timestampOne.AddHours(1), LogLevel.Error, 0, "Error message", null, (state, ex) => state);

            provider.IntervalControl.Resume();
            await provider.IntervalControl.Pause;

            Assert.Equal(
                "2016-05-04 03:02:01.000 +00:00 [Information] Cat: Info message" + Environment.NewLine +
                "2016-05-04 04:02:01.000 +00:00 [Error] Cat: Error message" + Environment.NewLine,
                File.ReadAllText(Path.Combine(TempPath, "LogFile.20160504.log")));
        }

        [Fact]
        public async Task RollsTextFile()
        {
            var provider = new TestFileLoggerProvider(TempPath);
            var logger = (BatchingLogger)provider.CreateLogger("Cat");

            await provider.IntervalControl.Pause;

            logger.Log(_timestampOne, LogLevel.Information, 0, "Info message", null, (state, ex) => state);
            logger.Log(_timestampOne.AddDays(1), LogLevel.Error, 0, "Error message", null, (state, ex) => state);

            provider.IntervalControl.Resume();
            await provider.IntervalControl.Pause;

            Assert.Equal(
                "2016-05-04 03:02:01.000 +00:00 [Information] Cat: Info message" + Environment.NewLine,
                File.ReadAllText(Path.Combine(TempPath, "LogFile.20160504.log")));

            Assert.Equal(
                "2016-05-05 03:02:01.000 +00:00 [Error] Cat: Error message" + Environment.NewLine,
                File.ReadAllText(Path.Combine(TempPath, "LogFile.20160505.log")));
        }

        [Fact]
        public async Task RespectsMaxFileCount()
        {
            Directory.CreateDirectory(TempPath);
            File.WriteAllText(Path.Combine(TempPath, "randomFile.txt"), "Text");

            var provider = new TestFileLoggerProvider(TempPath, maxRetainedFiles: 5);
            var logger = (BatchingLogger)provider.CreateLogger("Cat");

            await provider.IntervalControl.Pause;
            var timestamp = _timestampOne;

            for (int i = 0; i < 10; i++)
            {
                logger.Log(timestamp, LogLevel.Information, 0, "Info message", null, (state, ex) => state);
                logger.Log(timestamp.AddHours(1), LogLevel.Error, 0, "Error message", null, (state, ex) => state);

                timestamp = timestamp.AddDays(1);
            }

            provider.IntervalControl.Resume();
            await provider.IntervalControl.Pause;

            var actualFiles = new DirectoryInfo(TempPath)
                .GetFiles()
                .Select(f => f.Name)
                .OrderBy(f => f)
                .ToArray();

            Assert.Equal(new[] {
                "LogFile.20160509.log",
                "LogFile.20160510.log",
                "LogFile.20160511.log",
                "LogFile.20160512.log",
                "LogFile.20160513.log",
                "randomFile.txt"
            }, actualFiles);
        }
    }

    internal class TestFileLoggerProvider : FileLoggerProvider
    {
        internal ManualIntervalControl IntervalControl { get; } = new ManualIntervalControl();

        public TestFileLoggerProvider(
           string path,
           string fileName = "LogFile",
           int? maxFileSize = null,
           int? maxRetainedFiles = null,
           TimeSpan interval = default(TimeSpan),
           int? maxBatchSize = null,
           int? maxQueueSize = null) : base(
               path,
               fileName,
               maxFileSize,
               maxRetainedFiles,
               interval, maxBatchSize,
               maxQueueSize)
        {
        }

        protected override Task IntervalAsync(TimeSpan interval, CancellationToken cancellationToken)
        {
            return IntervalControl.IntervalAsync();
        }
    }

    public class BatchingLoggerProviderTests
    {
        DateTimeOffset _timestampOne = new DateTimeOffset(2016, 05, 04, 03, 02, 01, TimeSpan.Zero);
        string _nl = Environment.NewLine;

        [Fact]
        public async Task LogsInIntervals()
        {
            var provider = new TestBatchingLoggingProvider();
            var logger = (BatchingLogger)provider.CreateLogger("Cat");

            await provider.IntervalControl.Pause;

            logger.Log(_timestampOne, LogLevel.Information, 0, "Info message", null, (state, ex) => state);
            logger.Log(_timestampOne.AddHours(1), LogLevel.Error, 0, "Error message", null, (state, ex) => state);

            provider.IntervalControl.Resume();
            await provider.IntervalControl.Pause;

            Assert.Equal("2016-05-04 03:02:01.000 +00:00 [Information] Cat: Info message" + _nl, provider.Batches[0][0].Message);
            Assert.Equal("2016-05-04 04:02:01.000 +00:00 [Error] Cat: Error message" + _nl, provider.Batches[0][1].Message);
        }

        [Fact]
        public async Task RespectsBatchSize()
        {
            var provider = new TestBatchingLoggingProvider(maxBatchSize: 1);
            var logger = (BatchingLogger)provider.CreateLogger("Cat");

            await provider.IntervalControl.Pause;

            logger.Log(_timestampOne, LogLevel.Information, 0, "Info message", null, (state, ex) => state);
            logger.Log(_timestampOne.AddHours(1), LogLevel.Error, 0, "Error message", null, (state, ex) => state);

            provider.IntervalControl.Resume();
            await provider.IntervalControl.Pause;

            Assert.Equal(1, provider.Batches.Count);
            Assert.Equal(1, provider.Batches[0].Length);
            Assert.Equal("2016-05-04 03:02:01.000 +00:00 [Information] Cat: Info message" + _nl, provider.Batches[0][0].Message);

            provider.IntervalControl.Resume();
            await provider.IntervalControl.Pause;

            Assert.Equal(2, provider.Batches.Count);
            Assert.Equal(1, provider.Batches[1].Length);

            Assert.Equal("2016-05-04 04:02:01.000 +00:00 [Error] Cat: Error message" + _nl, provider.Batches[1][0].Message);
        }

        [Fact]
        public async Task BlocksWhenReachingMaxQueue()
        {
            var provider = new TestBatchingLoggingProvider(maxQueueSize: 1);
            var logger = (BatchingLogger)provider.CreateLogger("Cat");

            await provider.IntervalControl.Pause;

            logger.Log(_timestampOne, LogLevel.Information, 0, "Info message", null, (state, ex) => state);
            var task = Task.Run(() => logger.Log(_timestampOne.AddHours(1), LogLevel.Error, 0, "Error message", null, (state, ex) => state));

            Assert.False(task.Wait(1000));

            provider.IntervalControl.Resume();
            await provider.IntervalControl.Pause;

            Assert.Equal(1, provider.Batches.Count);
            Assert.Equal(1, provider.Batches[0].Length);
            Assert.Equal("2016-05-04 03:02:01.000 +00:00 [Information] Cat: Info message" + _nl, provider.Batches[0][0].Message);

            provider.IntervalControl.Resume();
            await provider.IntervalControl.Pause;

            Assert.Equal(2, provider.Batches.Count);
            Assert.Equal(1, provider.Batches[1].Length);
            Assert.Equal("2016-05-04 04:02:01.000 +00:00 [Error] Cat: Error message" + _nl, provider.Batches[1][0].Message);

            Assert.True(task.IsCompleted);
        }

        private class TestBatchingLoggingProvider: BatchingLoggerProvider
        {
            public List<LogMessage[]> Batches { get; } = new List<LogMessage[]>();
            public ManualIntervalControl IntervalControl { get; } = new ManualIntervalControl();

            public TestBatchingLoggingProvider(TimeSpan interval = default(TimeSpan), int? maxBatchSize = null, int? maxQueueSize = null)
                : base(interval == TimeSpan.Zero ? TimeSpan.FromSeconds(1) : interval, maxBatchSize, maxQueueSize)
            {
            }

            protected override Task WriteMessagesAsync(IEnumerable<LogMessage> messages)
            {
                Batches.Add(messages.ToArray());
                return Task.CompletedTask;
            }

            protected override Task IntervalAsync(TimeSpan interval, CancellationToken cancellationToken)
            {
                return IntervalControl.IntervalAsync();
            }
        }
    }


    internal class ManualIntervalControl
    {

        private TaskCompletionSource<object> _pauseCompletionSource = new TaskCompletionSource<object>();
        private TaskCompletionSource<object> _resumeCompletionSource;

        public Task Pause => _pauseCompletionSource.Task;

        public void Resume()
        {
            _pauseCompletionSource = new TaskCompletionSource<object>();
            _resumeCompletionSource.SetResult(null);
        }

        public async Task IntervalAsync()
        {
            _resumeCompletionSource = new TaskCompletionSource<object>();
            _pauseCompletionSource.SetResult(null);

            await _resumeCompletionSource.Task;
        }
    }
}
