// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.Logging.EventLog.Internal;

namespace Microsoft.Extensions.Logging.EventLog
{
    /// <summary>
    /// Settings for <see cref="EventLogLogger"/>.
    /// </summary>
    public class EventLogSettings
    {
        public const string DefaultLogName = "Application";
        public const string DefaultSourceName = "Application";
        public const string DefaultMachineName = ".";

        private string _logName;
        private string _sourceName;
        private string _machineName;

        /// <summary>
        /// Name of the event log. If <c>null</c> or not specified, "Application" is the default.
        /// </summary>
        public string LogName
        {
            get
            {
                return !string.IsNullOrEmpty(_logName) ? _logName : DefaultLogName;
            }
            set
            {
                _logName = value;
            }
        }

        /// <summary>
        /// Name of the event log source. If <c>null</c> or not specified, "Application" is the default.
        /// </summary>
        public string SourceName
        {
            get
            {
                return !string.IsNullOrEmpty(_sourceName) ? _sourceName : DefaultSourceName;
            }
            set
            {
                _sourceName = value;
            }
        }

        /// <summary>
        /// Name of the machine having the event log. If <c>null</c> or not specified, local machine is the default.
        /// </summary>
        public string MachineName
        {
            get
            {
                return !string.IsNullOrEmpty(_machineName) ? _machineName : DefaultMachineName;
            }
            set
            {
                _machineName = value;
            }
        }

        /// <summary>
        /// For unit testing purposes only.
        /// </summary>
        public IEventLog EventLog { get; set; }
    }
}
