// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.Logging.Console
{
    /// <summary>
    /// Settings for a <see cref="ConsoleLoggerProvider"/> taken from a configuration source.
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
    /// <para>
    /// The value of the IncludeScopes setting is taken from the IncludeScopes configuration property
    /// (of the <see cref="IConfiguration"/> passed to the constructor). 
    /// </para>
    /// <para>
    /// Checking <c>TryGetSwitch</c> takes values from the "LogLevel" subsection of the provided 
    /// configuration, checking for a configuration setting matching the category name and with the minimum 
    /// log level setting.
    /// </para>
    /// <para>
    /// If a category, or any of it's parents, is not present, then the value of the special 'Default'
    /// category is checked, and if it is not present, then the category is not logged.
    /// </para>
    /// </remarks>
    /// <example>
    /// Configures logging via a JSON configuration file, with a default level of <c>Warning</c>, 
    /// with <c>Information</c> level for namespaces starting with "CompanyB" (inherited by all child loggers), 
    /// and <c>Debug</c> level for any log messages from class "CompanyA.Namespace1.ClassB".
    /// <code>
    /// {
    ///  "IncludeScopes" : "false",
    ///  "LogLevel": {
    ///    "Default": "Warning",
    ///    "CompanyA.Namespace1.ClassB": "Debug",
    ///    "CompanyB": "Information"
    ///  }
    /// }
    /// </code>
    /// </example>
    public class ConfigurationConsoleLoggerSettings : IConsoleLoggerSettings
    {
        private readonly IConfiguration _configuration;

        public ConfigurationConsoleLoggerSettings(IConfiguration configuration)
        {
            _configuration = configuration;
            ChangeToken = configuration.GetReloadToken();
        }

        public IChangeToken ChangeToken { get; private set; }

        /// <summary>
        /// Gets whether log scope information should be displayed in the output, 
        /// from the value of the "IncludeScopes" configuration property,
        /// or false if it is not defined.
        /// </summary>
        public bool IncludeScopes
        {
            get
            {
                bool includeScopes;
                var value = _configuration["IncludeScopes"];
                if (string.IsNullOrEmpty(value))
                {
                    return false;
                }
                else if (bool.TryParse(value, out includeScopes))
                {
                    return includeScopes;
                }
                else
                {
                    var message = $"Configuration value '{value}' for setting '{nameof(IncludeScopes)}' is not supported.";
                    throw new InvalidOperationException(message);
                }
            }
        }

        public IConsoleLoggerSettings Reload()
        {
            ChangeToken = null;
            return new ConfigurationConsoleLoggerSettings(_configuration);
        }

        /// <summary>
        /// Gets the minimum log level for the specified category (or the special category 'Default'),
        /// from the "LogLevel" configuration section, checking for a configuration setting matching
        /// the category name and with the minimum log level setting.
        /// </summary>
        /// <param name="name">The configuration category to check.</param>
        /// <param name="level">The minimum <c>LogLevel</c> set for the category.</param>
        /// <returns>true if a <c>LogLevel</c> is set; false if it is not.</returns>
        /// <remarks>
        /// <para>
        /// Note that the console logger calls this for a category and all parent categories until
        /// a value is returned. If no value is returned, then the value for the special category 
        /// 'Default' is checked, and if that returns false then the category is not logged.
        /// </para>
        /// </remarks>
        public bool TryGetSwitch(string name, out LogLevel level)
        {
            var switches = _configuration.GetSection("LogLevel");
            if (switches == null)
            {
                level = LogLevel.None;
                return false;
            }

            var value = switches[name];
            if (string.IsNullOrEmpty(value))
            {
                level = LogLevel.None;
                return false;
            }
            else if (Enum.TryParse<LogLevel>(value, out level))
            {
                return true;
            }
            else
            {
                var message = $"Configuration value '{value}' for category '{name}' is not supported.";
                throw new InvalidOperationException(message);
            }
        }
    }
}
