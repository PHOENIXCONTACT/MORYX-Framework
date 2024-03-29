// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Container.Tests
{
    internal class Parent : INamedChildContainer<Child>
    {
        public string Name { get; set; }

        public Parent(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Get the child with this name for a specific component
        /// </summary>
        /// <param name="name">Name of the child or empty for child with the same name</param><param name="target">Target component the child is assigned to</param>
        public Child GetChild(string name, Type target)
        {
            return new Child(name, target) { Parent = this };
        }
    }

    internal class Child : IContainerChild<Parent>
    {
        public string Name { get; set; }
        public Type Target { get; set; }

        public Child(string name, Type target)
        {
            Name = name;
            Target = target;
        }

        /// <summary>
        /// Parent container of this child
        /// </summary>
        public Parent Parent { get; set; }
    }
}
