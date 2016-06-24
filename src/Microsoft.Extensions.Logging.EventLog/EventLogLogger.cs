// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.EventLog.Internal;

namespace Microsoft.Extensions.Logging.EventLog
{
    /// <summary>
    /// A logger that writes messages to Windows Event Log.
    /// </summary>
    public class EventLogLogger : IConfigurableLogger
    {
        private const string ContinuationString = "...";
        private readonly int _beginOrEndMessageSegmentSize;
        private readonly int _intermediateMessageSegmentSize;
        private Func<string, LogLevel, bool> _filter;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventLogLogger"/> class.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        public EventLogLogger(string name)
            : this(name, filter: (category, logLevel) => true, includeScopes: false)
        {
        }

        public EventLogLogger(string name, Func<string, LogLevel, bool> filter, bool includeScopes)
            : this(name,
                  filter: filter,
                  includeScopes: includeScopes,
                  eventLogSettings: new EventLogSettings())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventLogLogger"/> class.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        /// <param name="filter"></param>
        /// <param name="includeScopes"></param>
        /// <param name="eventLogSettings"></param>
        public EventLogLogger(string name, Func<string, LogLevel, bool> filter, bool includeScopes, EventLogSettings eventLogSettings)
            : this(name, filter, includeScopes,
                  eventLog: new WindowsEventLog(eventLogSettings.LogName, eventLogSettings.MachineName, eventLogSettings.SourceName))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventLogLogger"/> class.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        /// <param name="filter"></param>
        /// <param name="includeScopes"></param>
        /// <param name="eventLog"></param>
        public EventLogLogger(string name, Func<string, LogLevel, bool> filter, bool includeScopes, IEventLog eventLog)
        {
            Name = string.IsNullOrEmpty(name) ? nameof(EventLogLogger) : name;
            Filter = filter ?? ((category, logLevel) => true);
            IncludeScopes = includeScopes;

            // Due to the following reasons, we cannot have these checks either here or in IsEnabled method:
            // 1. Log name & source name existence check only works on local computer.
            // 2. Source name existence check requires Administrative privileges.
            EventLog = eventLog;

            // Examples:
            // 1. An error occu...
            // 2. ...esponse stream
            _beginOrEndMessageSegmentSize = EventLog.MaxMessageSize - ContinuationString.Length;

            // Example:
            // ...rred while writ...
            _intermediateMessageSegmentSize = EventLog.MaxMessageSize - 2 * ContinuationString.Length;
        }

        public string Name { get; set; }

        public Func<string, LogLevel, bool> Filter
        {
            get { return _filter; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _filter = value;
            }
        }

        public bool IncludeScopes { get; set; }

        public IEventLog EventLog { get; set; }

        /// <inheritdoc />
        public IDisposable BeginScope<TState>(TState state)
        {
            return NoopDisposable.Instance;
        }

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel)
        {
            return Filter == null || Filter(Name, logLevel);
        }

        /// <inheritdoc />
        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            message = Name + Environment.NewLine + message;

            if (exception != null)
            {
                message += Environment.NewLine + Environment.NewLine + exception.ToString();
            }

            WriteMessage(message, GetEventLogEntryType(logLevel), eventId.Id);
        }

        // category '0' translates to 'None' in event log
        private void WriteMessage(string message, EventLogEntryType eventLogEntryType, int eventId)
        {
            if (message.Length <= EventLog.MaxMessageSize)
            {
                EventLog.WriteEntry(message, eventLogEntryType, eventId, category: 0);
                return;
            }

            var startIndex = 0;
            string messageSegment = null;
            while (true)
            {
                // Begin segment
                // Example: An error occu...
                if (startIndex == 0)
                {
                    messageSegment = message.Substring(startIndex, _beginOrEndMessageSegmentSize) + ContinuationString;
                    startIndex += _beginOrEndMessageSegmentSize;
                }
                else
                {
                    // Check if rest of the message can fit within the maximum message size
                    // Example: ...esponse stream
                    if ((message.Length - (startIndex + 1)) <= _beginOrEndMessageSegmentSize)
                    {
                        messageSegment = ContinuationString + message.Substring(startIndex);
                        EventLog.WriteEntry(messageSegment, eventLogEntryType, eventId, category: 0);
                        break;
                    }
                    else
                    {
                        // Example: ...rred while writ...
                        messageSegment =
                            ContinuationString
                            + message.Substring(startIndex, _intermediateMessageSegmentSize)
                            + ContinuationString;
                        startIndex += _intermediateMessageSegmentSize;
                    }
                }

                EventLog.WriteEntry(messageSegment, eventLogEntryType, eventId, category: 0);
            }
        }

        private EventLogEntryType GetEventLogEntryType(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Information:
                case LogLevel.Debug:
                case LogLevel.Trace:
                    return EventLogEntryType.Information;
                case LogLevel.Warning:
                    return EventLogEntryType.Warning;
                case LogLevel.Critical:
                case LogLevel.Error:
                    return EventLogEntryType.Error;
                default:
                    return EventLogEntryType.Information;
            }
        }

        private class NoopDisposable : IDisposable
        {
            public static NoopDisposable Instance = new NoopDisposable();

            public void Dispose()
            {
            }
        }
    }
}
