// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Microsoft.Framework.Logging.Test
{
    public class TraceTypeEnumTest
    {
        private static readonly List<int> _values = Enum.GetValues(typeof(TraceType)).Cast<int>().ToList();

        [Fact]
        public static void EnumStartsAtOne()
        {
            Assert.Equal(_values.Min(), 1);
        }

        [Fact]
        public static void EnumValuesAreUniqueAndConsecutive()
        {
            _values.Sort();
            Assert.Equal(new[] { 1, 2, 3, 4, 5 }, _values);
        }
    }
}