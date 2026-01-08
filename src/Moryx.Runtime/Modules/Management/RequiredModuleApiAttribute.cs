// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Modules
{
    /// <summary>
    /// This attribute defines how the Runtime will handle dependencies between modules
    /// created by facade references. The two booleans create four different modes.
    /// 
    /// |   <see cref="IsStartDependency"/>   |   <see cref="IsOptional"/>   |   Behavior
    /// |-----------------------|----------------|-------------
    /// |       False           |      False     |   The module exporting the facade must be present, but does not affect the start of the dependent.
    /// |       True            |      False     |   The module exporting the facade must be present and does affect the start of the dependent.
    /// |       False           |      True      |   The module exporting the facade can be missing and does not affect the start of the dependent.
    /// |       True            |      True      |   The module exporting the facade can be missing, but does affect the start of the dependent if present.
    /// </summary>
    /// <example>
    /// [RequiredModuleApi(IsStartDependency = true, IsOptional = false)]
    /// public ISomeFacade SomeFacade { get; set; }
    /// </example>
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredModuleApiAttribute : Attribute
    {
        /// <summary>
        /// If this flag is <value>true</value> the Runtime will only start this module
        /// if and when the module that exports the referenced facade has reached the 
        /// <see cref="ServerModuleState.Running"/> state. If this flag is <value>false</value>
        /// both modules can be started and stopped independently.
        /// </summary>
        public bool IsStartDependency { get; set; }

        /// <summary>
        /// This flag indicates if at least one module must export the given facade. If this
        /// is set to <value>false</value> then the Runtime will abort the boot process if no
        /// module exports the facade. If set to <value>true</value> the Runtime will simply
        /// leave the property null if no host was found.
        /// </summary>
        public bool IsOptional { get; set; }
    }
}
