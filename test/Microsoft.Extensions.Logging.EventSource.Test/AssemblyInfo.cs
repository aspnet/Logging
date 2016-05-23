// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Xunit;

// The EventListener class has a bug that causes OnEventSourceCreated to be called before the listener
// is fully constructed. Unfortunately this behavor shipped in .NET Framework and now it cannot be changed
// because of application compatibility concerns.
//
// This means EventListener derivatives cannot rely on constructor parameters for initialization. Instead we use a static 
// variable to hold a ListenerSettings instance, and this is where our TestListener looks for its initialization data. 
//
// This in turn implies EventSourceLogger tests cannot be executed in parallel. We mark this assembly 
// with CollectionBehavior.CollectionPerAssembly to ensure that all tests in this assembly are executed serially.
[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]