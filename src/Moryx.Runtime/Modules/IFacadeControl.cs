// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Modules
{
    /// <summary>
    /// Interface for all facades used by ServerModuleBase
    /// </summary>
    public interface IFacadeControl
    {
        /// <summary>
        /// Delegate to validate if execution is currently allowed
        /// </summary>
        Action ValidateHealthState { get; set; }

        /// <summary>
        /// Module is starting and facade activated
        /// </summary>
        void Activate();

        /// <summary>
        /// Module is stopping and facade deactivates
        /// </summary>
        void Deactivate();
    }
}
