// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.Extensions.Logging
{
    public struct EventId
    {
        public EventId(int id, string name = null)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; }

        public string Name { get; }

        public static EventId operator +(EventId left, EventId right)
        {
            return new EventId(left.Id + right.Id);
        }

        public static EventId operator -(EventId left, EventId right)
        {
            return new EventId(left.Id - right.Id);
        }

        public static implicit operator EventId(int i)
        {
            return new EventId(i);
        }

        public override string ToString()
        {
            return Name ?? Id.ToString();
        }
    }
}
