// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.AzureAppServices;
using Microsoft.Extensions.Logging.AzureAppServices.Internal;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Extension methods for adding Azure diagnostics logger.
    /// </summary>
    public static class AzureAppServicesLoggerFactoryExtensions
    {
        /// <summary>
        /// Adds an Azure Web Apps diagnostics logger.
        /// </summary>
        /// <param name="builder">The extension method argument</param>
        public static ILoggingBuilder AddAzureWebAppDiagnostics(this ILoggingBuilder builder)
        {
            var context = WebAppContext.Default;
            if (context.IsRunningInAzureWebApp)
            {
                var config = new AzureConfigProvider().GetAzureLoggingConfiguration(context);

                builder.Services.AddSingleton<IConfigureOptions<AzureDiagnosticsFileLoggerOptions>, AzureFileLoggerConfigureOptions>();
                builder.Services.AddSingleton<IConfigureOptions<LoggerFilterOptions>>(CreateFileFilterConfigureOptions(config));

                builder.Services.AddSingleton<IConfigureOptions<LoggerFilterOptions>>(CreateBlobFilterConfigureOptions(config));

                builder.Services.Configure<AzureBlobLoggerConfigureOptions>(config);

                builder.Services.AddSingleton(context);

                // Only add the provider if we're in Azure WebApp. That cannot change once the apps started
                builder.Services.AddSingleton<ILoggerProvider, FileLoggerProvider>();
                builder.Services.AddSingleton<ILoggerProvider, AzureBlobLoggerProvider>();
            }

            return builder;
        }

        private static ConfigurationBasedLevelSwitcher CreateBlobFilterConfigureOptions(IConfiguration config)
        {
            return new ConfigurationBasedLevelSwitcher(
                configuration: config,
                provider: typeof(AzureBlobLoggerProvider),
                levelKey: "AzureBlobTraceLevel",
                enableKey: "AzureBlobEnabled");
        }

        private static ConfigurationBasedLevelSwitcher CreateFileFilterConfigureOptions(IConfiguration config)
        {
            return new ConfigurationBasedLevelSwitcher(
                configuration: config,
                provider: typeof(FileLoggerProvider),
                levelKey: "AzureDriveTraceLevel",
                enableKey: "AzureDriveEnabled");
        }

        /// <summary>
        /// Adds an Azure Web Apps diagnostics logger.
        /// </summary>
        /// <param name="factory">The extension method argument</param>
        public static ILoggerFactory AddAzureWebAppDiagnostics(this ILoggerFactory factory)
        {
            return AddAzureWebAppDiagnostics(factory, new AzureAppServicesDiagnosticsSettings());
        }

        /// <summary>
        /// Adds an Azure Web Apps diagnostics logger.
        /// </summary>
        /// <param name="factory">The extension method argument</param>
        /// <param name="settings">The setting object to configure loggers.</param>
        public static ILoggerFactory AddAzureWebAppDiagnostics(this ILoggerFactory factory, AzureAppServicesDiagnosticsSettings settings)
        {
            var context = WebAppContext.Default;
            if (context.IsRunningInAzureWebApp)
            {
                var config = new AzureConfigProvider().GetAzureLoggingConfiguration(context);

                // Only add the provider if we're in Azure WebApp. That cannot change once the apps started
                var fileOptions = new OptionsManager<AzureDiagnosticsFileLoggerOptions>(
                    new IConfigureOptions<AzureDiagnosticsFileLoggerOptions>[]
                    {
                        new AzureFileLoggerConfigureOptions(context),
                        new ConfigureOptions<AzureDiagnosticsFileLoggerOptions>(options =>
                        {
                            options.FileSizeLimit = settings.FileSizeLimit;
                            options.RetainedFileCountLimit = settings.RetainedFileCountLimit;
                            options.BackgroundQueueSize = settings.BackgroundQueueSize;
                            if (settings.FileFlushPeriod != null)
                            {
                                options.FlushPeriod = settings.FileFlushPeriod.Value;
                            }
                        })
                    }
                    );

                var blobOptions = new OptionsManager<AzureDiagnosticsBlobLoggerOptions>(
                    new IConfigureOptions<AzureDiagnosticsBlobLoggerOptions>[] {
                        new AzureBlobLoggerConfigureOptions(config),
                        new ConfigureOptions<AzureDiagnosticsBlobLoggerOptions>(options =>
                        {
                            options.BlobName = settings.BlobName;
                            options.BackgroundQueueSize = settings.BackgroundQueueSize;
                            options.FlushPeriod = settings.BlobCommitPeriod;
                            options.BatchSize = settings.BlobBatchSize;
                        })
                    }
                    );

                var filterOptions = new OptionsMonitor<LoggerFilterOptions>(
                    new []
                    {
                        CreateFileFilterConfigureOptions(config),
                        CreateBlobFilterConfigureOptions(config)
                    },
                    new [] { new ConfigurationChangeTokenSource<LoggerFilterOptions>(config) });

                factory.AddProvider(new ForwardingLoggerProvider(
                    new LoggerFactory(
                        new ILoggerProvider[]
                        {
                            new FileLoggerProvider(fileOptions),
                            new AzureBlobLoggerProvider(blobOptions)
                        },
                        filterOptions
                        )
                    ));
            }
            return factory;
        }

        internal class ForwardingLoggerProvider : ILoggerProvider
        {
            private readonly ILoggerFactory _loggerFactory;

            public ForwardingLoggerProvider(ILoggerFactory loggerFactory)
            {
                _loggerFactory = loggerFactory;
            }

            public void Dispose()
            {
                _loggerFactory.Dispose();
            }

            public ILogger CreateLogger(string categoryName)
            {
                return _loggerFactory.CreateLogger(categoryName);
            }
        }
    }
}
