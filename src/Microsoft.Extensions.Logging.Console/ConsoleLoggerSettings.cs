// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.Logging.Console
{
    /// <summary>
    /// Basic settings for a <see cref="ConsoleLoggerProvider"/> that can configured in code.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When <see cref="IConsoleLoggerSettings"/> are defined, then only the categories 
    /// they return a result from <c>TryGetSwitch</c>, either for themselves or for a parent, 
    /// are logged.
    /// </para>
    /// <para>
    /// If a category is not configured then the value of the special category 'Default' (case sensitive) 
    /// is used, or 'Default' is not specified then they are not logged at all. 
    /// </para>
    /// </remarks>
    /// <example>
    /// Configures logging via code, with a default level of <c>Warning</c>, 
    /// with <c>Information</c> level for namespaces starting with "CompanyB" (inherited by all child loggers), 
    /// and <c>Debug</c> level for any log messages from class "CompanyA.Namespace1.ClassB".
    /// <code>
    /// var consoleSettings = new ConsoleLoggerSettings();
    /// consoleSettings.Switches = new Dictionary&lt;string, LogLevel&gt;() {
    ///     { "Default", LogLevel.Warning },
    ///     { "CompanyA.Namespace1.ClassB", LogLevel.Debug },
    ///     { "CompanyB", LogLevel.Information },
    /// };
    /// factory.AddConsole(consoleSettings);
    /// </code>
    /// </example>
    public class ConsoleLoggerSettings : IConsoleLoggerSettings
    {
        public IChangeToken ChangeToken { get; set; }

        /// <summary>
        /// Gets or sets whether log scope information should be displayed in the output.
        /// </summary>
        public bool IncludeScopes { get; set; }

        /// <summary>
        /// Gets or sets a dictionary of named categories (or the special category 'Default') 
        /// with the minimum log level for that category and inherited by child categories.
        /// </summary>
        public IDictionary<string, LogLevel> Switches { get; set; } = new Dictionary<string, LogLevel>();

        public IConsoleLoggerSettings Reload()
        {
            return this;
        }

        /// <inheritdoc/>
        public bool TryGetSwitch(string name, out LogLevel level)
        {
            return Switches.TryGetValue(name, out level);
        }
    }
}
