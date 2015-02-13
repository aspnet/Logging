﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Framework.Logging
{
    public class NullLogger : ILogger
    {
        public static NullLogger Instance = new NullLogger();

        public IDisposable BeginScope(object state)
        {
            return NullDisposable.Instance;
        }

        public void Write(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, bool, string> formatter)
        {
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }
    }
}