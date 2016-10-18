// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.Logging.Filter
{
    /// <summary>
    /// Filter settings for messages logged by an <see cref="ILogger"/>.
    /// </summary>
    public class ConfigurationFilterLoggerSettings : IFilterLoggerSettings
    {
        private readonly IConfiguration _configuration;

        public ConfigurationFilterLoggerSettings(IConfiguration configuration)
        {
            _configuration = configuration;
            ChangeToken = configuration.GetReloadToken();
        }

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

        public IFilterLoggerSettings Reload()
        {
            ChangeToken = null;
            return new ConfigurationFilterLoggerSettings(_configuration);
        }

        public IChangeToken ChangeToken { get; private set; }
    }
}