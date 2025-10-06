// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;

namespace Moryx.ControlSystem.Processes
{
    /// <summary>
    /// Standard interface for groups of types derived from <see cref="IProcessHolderPosition"/>
    /// </summary>
    public interface IProcessHolderGroup : IResource
    {
        /// <summary>
        /// All positions of the group
        /// </summary>
        IEnumerable<IProcessHolderPosition> Positions { get; }

        /// <summary>
        /// The entire group is reset
        /// </summary>
        void Reset();
    }

    /// <summary>
    /// Generic interface for groups of types derived from <see cref="IProcessHolderPosition"/>
    /// </summary>
    /// <typeparam name="TPosition"></typeparam>
    public interface IProcessHolderGroup<out TPosition> : IProcessHolderGroup
        where TPosition : IProcessHolderPosition
    {
        /// <summary>
        /// All positions of the group
        /// </summary>
        new IEnumerable<TPosition> Positions { get; }
    }
}
