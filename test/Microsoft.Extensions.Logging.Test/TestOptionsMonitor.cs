// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging.Test
{
    public static class TestOptionsMonitor
    {
        public static TestOptionsMonitor<T> Create<T>(T options) => new TestOptionsMonitor<T>(options);
    }

    public class TestOptionsMonitor<T> : IOptionsMonitor<T>
    {
        private T _options;
        private event Action<T, string> _onChange;

        public TestOptionsMonitor(T options)
        {
            _options = options;
        }

        public T Get(string name) => _options;

        public IDisposable OnChange(Action<T, string> listener)
        {
            _onChange += listener;
            return null;
        }

        public T CurrentValue => _options;

        public void Set(T options)
        {
            _options = options;
            NotifyChanged();
        }

        // Useful when the change is just done by changing the options object in-place rather than specifying a new one.
        public void NotifyChanged()
        {
            _onChange?.Invoke(_options, "");
        }
    }
}
