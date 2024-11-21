// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Container.Tests
{
    [Component(LifeCycle.Singleton)]
    public class Component
    {
        public IDependency Unnamed { get; set; }

        [Named("DepA")]
        public IDependency DepA { get; set; }

        [Named("DepB")]
        public IDependency DepB { get; set; }
    }

    public class Impossible
    {
        [Named("DepC")]
        public IDependency DepC { get; set; }
    }

    public interface IDependency
    {
        string GetName();
    }

    public class DependencyA : IDependency
    {
        public string GetName()
        {
            return "DepA";
        }
    }

    public class DependencyB : IDependency
    {
        public string GetName()
        {
            return "DepB";
        }
    }
}
