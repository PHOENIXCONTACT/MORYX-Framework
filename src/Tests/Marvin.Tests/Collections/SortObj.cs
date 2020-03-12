// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Collections;

namespace Marvin.Tests.Collections
{
    internal class SortObj : ISortableObject
    {
        public SortObj(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public int SortOrder { get; set; }

        public override string ToString()
        {
            return $"Order: {SortOrder} - Name: {Name}";
        }
    }
}
