// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.Logging.Debug;
using Xunit;

namespace Microsoft.Extensions.Logging
{
    public class DebugLoggerTest
    {
        private const string _categoryName = "test";

        [Fact]
        public void CallingBeginScopeOnLogger_AlwaysReturnsNewDisposableInstance()
        {
            // Arrange
            var sink = new DebugSink();

            // Act
            var disposable1 = sink.BeginScope(_categoryName, "Scope1");
            var disposable2 = sink.BeginScope(_categoryName, "Scope2");

            // Assert
            Assert.NotNull(disposable1);
            Assert.NotNull(disposable2);
            Assert.NotSame(disposable1, disposable2);
        }

        [Fact]
        public void CallingBeginScopeOnLogger_ReturnsNonNullableInstance()
        {
            // Arrange
            var logger = new DebugSink();

            // Act
            var disposable = logger.BeginScope(_categoryName, "Scope1");

            // Assert
            Assert.NotNull(disposable);
        }
    }
}
