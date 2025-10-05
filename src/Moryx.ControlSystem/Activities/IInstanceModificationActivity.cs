// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Identity;

namespace Moryx.ControlSystem.Activities
{
    /// <summary>
    /// Activity that modifies the product instance it operates on. Those modifications include creating, loading and changing the instance.
    /// Creating and loading are performed post-completion while property changes shall be performed by the resource
    /// </summary>
    public interface IInstanceModificationActivity : IActivity
    {
        /// <summary>
        /// (Optional) identity of the instance for <see cref="InstanceModificationType.Loaded"/>
        /// </summary>
        IIdentity InstanceIdentity { get; set; }

        /// <summary>
        /// Modification that was performed on the product instance depending on the activities result
        /// </summary>
        InstanceModificationType  ModificationType { get; set; }
    }
}
