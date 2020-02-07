// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Maintenance.Modules
{
    /// <summary>
    /// Different levels to diffentiate the severity of a message (used only for webservice communication)
    /// </summary>
    public enum ModuleNotificationType
    {
        /// <summary>
        /// Information about occured events that are nether a warning nor an error
        /// </summary>
        Info,

        /// <summary>
        /// Events that may destabilize the component
        /// </summary>
        Warning,

        /// <summary>
        /// Critical events that may obstruct any further execution
        /// </summary>
        Failure
    }
}
