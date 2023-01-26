// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Container.Tests
{
    internal class FakeAutoInstaller : AutoInstaller
    {
        /// <summary>
        /// Create a new instance of the <see cref="AutoInstaller"/> for this assembly 
        /// </summary>
        public FakeAutoInstaller() : base(typeof(FakeAutoInstaller).Assembly)
        {
        }

        public bool SkippedNamed { get; private set; }
        public bool SkippedUnnamed { get; private set; }

        /// <summary>
        /// Install all components
        /// </summary>
        public override void Install(IComponentRegistrator registrator)
        {
            SkippedNamed = !ShallInstall(registrator, typeof(NamedDummy));

            SkippedUnnamed = !ShallInstall(registrator, typeof(UnnamedDummy));
        }
    }
}
