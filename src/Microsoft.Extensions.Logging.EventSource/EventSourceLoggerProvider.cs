// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.Tracing;

namespace Microsoft.Extensions.Logging.EventSourceLogger
{
    /// <summary>
    /// The provider for the <see cref="EventSourceLogger"/>.
    /// </summary>
    internal class EventSourceLoggerProvider : ILoggerProvider
    {
        // A small integer that uniquely identifies the LoggerFactory assoicated with this LoggingProvider.
        // Zero is illegal (it means we are uninitialized), and have to be added to the factory. 
        private int _factoryID;

        private ILoggerFactory _loggerFactory;
        private LogLevel _defaultLevel;
        private string _filterSpec;
        private EventSourceLogger _loggers; // Linked list of loggers that I have created

        public EventSourceLoggerProvider(ILoggerFactory loggerFactory, EventSourceLoggerProvider next = null)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }
            _loggerFactory = loggerFactory;
            Next = next;
        }

        public readonly EventSourceLoggerProvider Next;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public ILogger CreateLogger(string categoryName)
        {
            var newLogger = _loggers = new EventSourceLogger(categoryName, _factoryID, _loggers);
            ParseLevelSpecs(_filterSpec, _defaultLevel, newLogger.CategoryName, out newLogger.Level);
            return newLogger;
        }

        public void Dispose()
        {
            SetFilterSpec(null); // Turn off any logging
        }

        // Sets the filtering for a particular 
        public void SetFilterSpec(string filterSpec)
        {
            _filterSpec = filterSpec;
            _defaultLevel = GetDefaultLevel();

            // Update the levels of all the loggers to match what the filter specification asks for.   
            for (var logger = _loggers; logger != null; logger = logger.Next)
            {
                ParseLevelSpecs(filterSpec, _defaultLevel, logger.CategoryName, out logger.Level);
            }

            if (filterSpec != null && _factoryID == 0)
            {
                // Compute an ID for the Factory.  It is its position in the list (starting at 1, we reserve 0 to mean unstarted). 
                _factoryID = 1;
                for (var cur = Next; cur != null; cur = cur.Next)
                {
                    _factoryID++;
                }

                // Add myself to the factory.  Now my CreateLogger methods will be called.  
                _loggerFactory.AddProvider(this);
            }
        }

        private LogLevel GetDefaultLevel()
        {
            var allMessageKeywords = LoggingEventSource.Keywords.Message | LoggingEventSource.Keywords.FormattedMessage | LoggingEventSource.Keywords.JsonMessage;

            if (LoggingEventSource.Instance.IsEnabled(EventLevel.Informational, allMessageKeywords))
            {
                if (LoggingEventSource.Instance.IsEnabled(EventLevel.Verbose, allMessageKeywords))
                {
                    return LogLevel.Debug;
                }
                else
                {
                    return LogLevel.Information;
                }
            }
            else
            {
                if (LoggingEventSource.Instance.IsEnabled(EventLevel.Warning, allMessageKeywords))
                {
                    return LogLevel.Warning;
                }
                else
                {
                    if (LoggingEventSource.Instance.IsEnabled(EventLevel.Error, allMessageKeywords))
                    {
                        return LogLevel.Error;
                    }
                    else
                    {
                        return LogLevel.Critical;
                    }
                }
            }
        }

        /// <summary>
        /// Given a set of specifications  Pat1:Level1;Pat1;Level2 ... Where
        /// Pat is a string pattern (a logger Name with a optional trailing wildcard * char)
        /// and Level is a number 0 (Trace) through 5 (Critical).  
        /// 
        /// The :Level can be omitted (thus Pat1;Pat2 ...) in which case the level is 1 (Debug). 
        /// 
        /// A completely emtry sting act like * (all loggers set to Debug level).  
        /// 
        /// The first speciciation that 'loggers' Name matches is used.   
        /// </summary>
        private void ParseLevelSpecs(string filterSpec, LogLevel defaultLevel, string loggerName, out LogLevel level)
        {
            if (filterSpec == null)
            {
                level = LoggingEventSource.LoggingDisabled;      // Null means disable.  
                return;
            }
            if (filterSpec == string.Empty)
            {
                level = defaultLevel;
                return;
            }
            level = LoggingEventSource.LoggingDisabled;   // If the logger does not match something, it is off. 

            // See if logger.Name  matches a _filterSpec pattern.  
            int namePos = 0;
            int specPos = 0;
            for (;;)
            {
                if (namePos < loggerName.Length)
                {
                    if (filterSpec.Length <= specPos)
                    {
                        break;
                    }

                    char specChar = filterSpec[specPos++];
                    char nameChar = loggerName[namePos++];
                    if (specChar == nameChar)
                    {
                        continue;
                    }

                    // We allow wildcards a the end.  
                    if (specChar == '*' && ParseLevel(defaultLevel, filterSpec, specPos, ref level))
                    {
                        return;
                    }
                }
                else if (ParseLevel(defaultLevel, filterSpec, specPos, ref level))
                {
                    return;
                }

                // Skip to the next spec in the ; separated list.  
                specPos = filterSpec.IndexOf(';', specPos) + 1;
                if (specPos <= 0) // No ; done. 
                {
                    break;
                }
                namePos = 0;    // Reset where we are searching in the name.  
            }
        }

        /// <summary>
        /// Parses the level specification (which should look like :N where n is a  number 0 (Trace)
        /// through 5 (Critical).   It can also be an empty string (which means 1 (Debug) and ';' marks 
        /// the end of the specifcation This specification should start at spec[curPos]
        /// It returns the value in 'ret' and returns true if successful.  If false is returned ret is left unchanged.  
        /// </summary>
        private bool ParseLevel(LogLevel defaultLevel, string spec, int specPos, ref LogLevel ret)
        {
            int endPos = spec.IndexOf(';', specPos);
            if (endPos < 0)
            {
                endPos = spec.Length;
            }

            if (specPos == endPos)
            {
                // No :Num spec means Debug
                ret = defaultLevel;
                return true;
            }
            if (spec[specPos++] != ':')
            {
                return false;
            }

            string levelStr = spec.Substring(specPos, endPos - specPos);
            int level;
            switch (levelStr)
            {
                case "Trace":
                    ret = LogLevel.Trace;
                    break;
                case "Debug":
                    ret = LogLevel.Debug;
                    break;
                case "Information":
                    ret = LogLevel.Information;
                    break;
                case "Warning":
                    ret = LogLevel.Warning;
                    break;
                case "Error":
                    ret = LogLevel.Error;
                    break;
                case "Critical":
                    ret = LogLevel.Critical;
                    break;
                default:
                    if (!int.TryParse(levelStr, out level))
                    {
                        return false;
                    }
                    if (!(LogLevel.Trace <= (LogLevel)level && (LogLevel)level <= LogLevel.None))
                    {
                        return false;
                    }
                    ret = (LogLevel)level;
                    break;
            }
            return true;
        }
    }
}
