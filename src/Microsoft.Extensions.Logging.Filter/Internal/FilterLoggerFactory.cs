// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.Extensions.Logging.Filter.Internal
{
    public class FilterLoggerFactory : ILoggerFactory
    {
        private readonly ILoggerFactory _innerLoggerFactory;
        private readonly IFilterLoggerSettings _settings;

        public FilterLoggerFactory(ILoggerFactory innerLoggerFactory, IFilterLoggerSettings settings)
        {
            _innerLoggerFactory = innerLoggerFactory;
            _settings = settings;
        }

        public void AddProvider(ILoggerProvider provider)
        {
            var wrappedProvider = new FilterLoggerProvider(provider, _settings);
            _innerLoggerFactory.AddProvider(wrappedProvider);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _innerLoggerFactory.CreateLogger(categoryName);
        }

        public void Dispose()
        {
            _innerLoggerFactory.Dispose();
        }
    }
}
