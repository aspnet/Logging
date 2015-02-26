// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Framework.Logging
{
    /// <summary>
    /// A generic interface for logging where the category name is taken from the specified TCategoryName.
    /// For enabling activation of named ILogger from DI.
    /// </summary>
    public interface ILogger<TCategoryName> : ILogger
    {
        
    }
}
