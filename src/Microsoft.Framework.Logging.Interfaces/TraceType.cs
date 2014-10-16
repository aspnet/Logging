// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.Framework.Logging
{
#if ASPNET50 || ASPNETCORE50
    [Runtime.AssemblyNeutral]
#endif
    public enum TraceType
    {
        Critical = 5,
        Error = 4,
        Warning = 3,
        Information = 2,
        Verbose = 1,
    }
}
