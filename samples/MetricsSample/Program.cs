using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MetricsSample
{
    class Program
    {
        private static readonly int ThreadCount = 100;

        static void Main(string[] args)
        {
            // A Web App based program would configure logging via the WebHostBuilder.
            // Create a logger factory with filters that can be applied across all logger providers.
            var serviceCollection = new ServiceCollection()
                .AddLogging(builder =>
                {
                    builder.AddConsole();
                    builder.Services.AddSingleton<ILoggerProvider, SampleConsoleMetricsLoggerProvider>();
                });

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            // Spin up a whole bunch of "threads"
            var cts = new CancellationTokenSource();
            var tasks = new Task[ThreadCount];
            var metric = logger.DefineMetric("RandomValue");
            for (var i = 0; i < ThreadCount; i += 1)
            {
                tasks[i] = Task.Factory.StartNew(() => RunWorker(i, metric, cts.Token), TaskCreationOptions.LongRunning);
            }

            cts.CancelAfter(TimeSpan.FromSeconds(10));

            Task.WaitAll(tasks);
        }

        private static void RunWorker(int i, IMetric metric, CancellationToken cancellationToken)
        {
            var rando = new Random();
            while (!cancellationToken.IsCancellationRequested)
            {
                metric.RecordValue(rando.Next(1, 100));
            }
        }
    }
}
