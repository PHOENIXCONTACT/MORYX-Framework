// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Modules;

namespace Moryx.Notifications
{
    /// <summary>
    /// Facade interface for providing notifications
    /// </summary>
    public interface INotificationSource : INotificationSourceAdapter, ILifeCycleBoundFacade
    {
        /// <summary>
        /// Name of the Source which will publish notifications
        /// </summary>
        string Name { get; }
    }
}
