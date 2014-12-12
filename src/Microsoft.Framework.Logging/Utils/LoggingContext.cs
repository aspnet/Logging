// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
#if ASPNET50 || ASPNETCORE50
#if ASPNET50
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
#endif
using System;
using System.Threading;

namespace Microsoft.Framework.Logging
{
    public class LoggingContext : IDisposable
    {
        public LoggingContext(Guid requestId)
        {
            RequestId = requestId;
            Current = this;
        }

        public Guid RequestId { get; private set; }

#if ASPNET50
        private static string FieldKey = typeof(LoggingContext).FullName + ".Value";
        public static LoggingContext Current
        {
            get
            {
                var handle = CallContext.LogicalGetData(FieldKey) as ObjectHandle;

                if (handle == null)
                {
                    return default(LoggingContext);
                }

                return (LoggingContext)handle.Unwrap();
            }
            set
            {
                CallContext.LogicalSetData(FieldKey, new ObjectHandle(value));
            }
        }
#else
        private static AsyncLocal<LoggingContext> _value = new AsyncLocal<LoggingContext>();
        public static LoggingContext Current
        {
            set
            {
                _value.Value = value;
            }
            get
            {
                return _value.Value;
            }
        }
#endif
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Current = null;
                RequestId = Guid.Empty;
            }
        }
    }
}
#endif