// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.Logging.Filtering
{
    /// <summary>
    /// A namespace aware <see cref="ILogFilter"/>.
    /// </summary>
    /// <remarks>
    /// If the log entry does not match any of the configured patterns,
    /// the log entry will NOT be sent to the sink.
    /// </remarks>
    public class LogFilter : ILogFilter
    {
        public const string DefaultCategory = "Default";

        private readonly ILogFilterSettings _settings;

        public LogFilter(ILogFilterSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            _settings = settings;
        }

        public bool IsEnabled(ILogSink sink, string categoryName, LogLevel level)
        {
            Type sinkType = sink.GetType();

            foreach (var prefix in GetKeyPrefixes(categoryName))
            {
                LogLevel prefixLevel;
                if (_settings.TryGetSwitch(sinkType, prefix, out prefixLevel))
                {
                    return level >= prefixLevel;
                }
            }

            return false;
        }

        private IEnumerable<string> GetKeyPrefixes(string name)
        {
            while (!string.IsNullOrEmpty(name))
            {
                yield return name;
                var lastIndexOfDot = name.LastIndexOf('.');
                if (lastIndexOfDot == -1)
                {
                    yield return DefaultCategory;
                    break;
                }
                name = name.Substring(0, lastIndexOfDot);
            }
        }
    }
}
