// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Framework.Logging.Infrastructure
{
    public class NullLogicalOperation : IDisposable
    {
        public static readonly NullLogicalOperation Instance = new NullLogicalOperation();

        public void Dispose()
        {
            // intentionally does nothing
        }
    }
}