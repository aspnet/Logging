using System;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging.AzureAppServices.Test
{
    internal class OptionsWrapperMonitor<T> : IOptionsMonitor<T>
    {
        public OptionsWrapperMonitor(T currentValue)
        {
            CurrentValue = currentValue;
        }

        public IDisposable OnChange(Action<T> listener)
        {
            return null;
        }

        public T CurrentValue { get; }
    }
}