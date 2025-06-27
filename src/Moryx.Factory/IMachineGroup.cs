// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;

namespace Moryx.Factory
{
    /// <summary>
    /// Group of resources inside the factory
    /// </summary>
    public interface IMachineGroup : IResource
    {
        /// <summary>
        /// Default icon for this resource group
        /// </summary>
        string DefaultIcon { get; set; }
    }
}
