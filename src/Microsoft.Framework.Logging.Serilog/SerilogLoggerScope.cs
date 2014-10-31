using System;
#if ASPNET50
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
#endif

namespace Microsoft.Framework.Logging.Serilog
{
    public class SerilogLoggerScope : IDisposable
    {
        private readonly SerilogLoggerProvider _provider;

        public SerilogLoggerScope(SerilogLoggerProvider provider, SerilogLoggerScope parent, string name, object state)
        {
            _provider = provider;
            Name = name;
            State = state;

            Parent = _provider.CurrentScope;
            _provider.CurrentScope = this;
        }

        public SerilogLoggerScope Parent { get; private set; }
        public string Name { get; private set; }
        public object State { get; private set; }

        public void RemoveScope()
        {
            for (var scan = _provider.CurrentScope; scan != null; scan = scan.Parent)
            {
                if (ReferenceEquals(scan, this))
                {
                    _provider.CurrentScope = Parent;
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    RemoveScope();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources. 
        // ~ScopeDisposable() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}