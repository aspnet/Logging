// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Filter;
using Xunit;

namespace Microsoft.Extensions.Logging.Test
{
    public class ConfigurationFilterLoggerSettingsTest
    {
        [Fact]
        public void TryGetSwitch_OnValidConfiguration_LoadsValueFromConfiguration()
        {
            // Arrange
            var dict = new Dictionary<string, string>
            {
                ["Logging:LogLevel:System"] = "Information"
            };
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(dict)
                .Build();
            var settings = new ConfigurationFilterLoggerSettings(config.GetSection("Logging"));

            // Act
            LogLevel level;
            var success = settings.TryGetSwitch("System", out level);

            // Assert
            Assert.True(success);
            Assert.Equal(LogLevel.Information, level);
        }

        [Fact]
        public void TryGetSwitch_OnMissingLogLevelSection_ReturnsLogLevelNone()
        {
            // Arrange
            var dict = new Dictionary<string, string>
            {
                ["Logging:"] = ""
            };
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(dict)
                .Build();
            var settings = new ConfigurationFilterLoggerSettings(config.GetSection("Logging"));

            // Act
            LogLevel level;
            var success = settings.TryGetSwitch("System", out level);

            // Assert
            Assert.False(success);
            Assert.Equal(LogLevel.None, level);
        }

        [Fact]
        public void TryGetSwitch_OnMissingSwitch_ReturnsLogLevelNone()
        {
            // Arrange
            var dict = new Dictionary<string, string>
            {
                ["Logging:LogLevel:System"] = "Information"
            };
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(dict)
                .Build();
            var settings = new ConfigurationFilterLoggerSettings(config.GetSection("Logging"));

            // Act
            LogLevel level;
            var success = settings.TryGetSwitch("Microsoft", out level);

            // Assert
            Assert.False(success);
            Assert.Equal(LogLevel.None, level);
        }

        [Fact]
        public void TryGetSwitch_IfLevelNullOrEmpty_ReturnsLogLevelNone()
        {
            // Arrange
            var dict = new Dictionary<string, string>
            {
                ["Logging:LogLevel:System"] = ""
            };
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(dict)
                .Build();
            var settings = new ConfigurationFilterLoggerSettings(config.GetSection("Logging"));

            // Act
            LogLevel level;
            var success = settings.TryGetSwitch("System", out level);

            // Assert
            Assert.False(success);
            Assert.Equal(LogLevel.None, level);
        }

        [Fact]
        public void TryGetSwitch_IfInvalidEnumValue_ThrowsException()
        {
            // Arrange
            var dict = new Dictionary<string, string>
            {
                ["Logging:LogLevel:System"] = "SomethingStrange"
            };
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(dict)
                .Build();
            var settings = new ConfigurationFilterLoggerSettings(config.GetSection("Logging"));

            // Act Assert
            LogLevel level;
            Assert.Throws<InvalidOperationException>(() => settings.TryGetSwitch("System", out level));
        }

        [Fact]
        public void Reload_ReturnsNewObject()
        {
            // Arrange
            var dict = new Dictionary<string, string>
            {
                ["Logging:LogLevel:System"] = "SomethingStrange"
            };
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(dict)
                .Build();
            var settings = new ConfigurationFilterLoggerSettings(config.GetSection("Logging"));

            // Act Assert
            var newSettings = settings.Reload();

            Assert.NotNull(newSettings);
            Assert.NotSame(settings, newSettings);
            Assert.IsType<ConfigurationFilterLoggerSettings>(newSettings);

        }
    }
}