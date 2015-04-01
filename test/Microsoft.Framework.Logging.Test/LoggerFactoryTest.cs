// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Framework.Logging;
using Xunit;

namespace Microsoft.Framework.Logging.Test
{
    public class LoggerFactoryTest
    {
        [Fact]
        public static void Dispose_ProvidersAreDisposed()
        {
            // Arrange
            var factory = new LoggerFactory();
            var provider = new DisposableProvider();
            factory.AddProvider(provider);

            // Act
            factory.Dispose();

            // Assert
            Assert.True(provider.Disposed);
        }

        private class DisposableProvider : ILoggerProvider, IDisposable
        {
            public bool Disposed { get; set; }

            public ILogger CreateLogger(string name)
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
                Disposed = true;
            }

        }
    }
}