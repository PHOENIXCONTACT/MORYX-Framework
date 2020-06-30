// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Marvin.Modules
{
    /// <summary>
    /// Base interface for all major components of applications within the Marvin platform application family.
    /// Each module offers a certain functionality to the user or other modules. 
    /// It may contain components and exchangable plugins to increase reuse, customization and flexibility. 
    /// This does only apply to level 1 components like a ServerModule or ClientModule.
    /// </summary>
    public interface IModule : IInitializable
    {
        /// <summary>
        /// Unique name for this module within the platform it is designed for
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Notifications published by this module
        /// </summary>
        INotificationCollection Notifications { get; } 
    }
}
