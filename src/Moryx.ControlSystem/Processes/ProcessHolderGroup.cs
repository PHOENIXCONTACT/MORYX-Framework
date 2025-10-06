// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;

namespace Moryx.ControlSystem.Processes
{
    /// <summary>
    /// Default implementation for <see cref="IProcessHolderGroup{IProcessHolderPosition}"/>
    /// </summary>
    public class ProcessHolderGroup : Resource, IProcessHolderGroup
    {
        /// <summary>
        /// All positions of this carrier
        /// </summary>
        [ReferenceOverride(nameof(Children))]
        public IReferences<IProcessHolderPosition> Positions { get; set; }

        /// <inheritdoc />
        IEnumerable<IProcessHolderPosition> IProcessHolderGroup.Positions => Positions;

        /// <inheritdoc />
        public void Reset()
        {
            foreach (var position in Positions)
            {
                position.Reset();
            }
        }
    }
}
