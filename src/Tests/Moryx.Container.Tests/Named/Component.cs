// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Container.Tests
{
    [Registration(LifeCycle.Singleton)]
    internal class Component
    {
        public IDependency Unnamed { get; set; }

        [Named("DepA")]
        public IDependency DepA { get; set; }

        [Named("DepB")]
        public IDependency DepB { get; set; }
    }

    internal class Impossible
    {
        [Named("DepC")]
        public IDependency DepC { get; set; }
    }

    internal interface IDependency
    {
        string GetName();
    }

    internal class DependencyA : IDependency
    {
        public string GetName()
        {
            return "DepA";
        }
    }

    internal class DependencyB : IDependency
    {
        public string GetName()
        {
            return "DepB";
        }
    }
}
