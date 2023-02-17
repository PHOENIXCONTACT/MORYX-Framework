// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Container
{
    /// <summary>
    /// Interface for all installers that extend the <see cref="IContainer"/>
    /// </summary>
    public interface IContainerInstaller
    {
        /// <summary>
        /// Install using the given registrator
        /// </summary>
        void Install(IComponentRegistrator registrator);
    }
}
