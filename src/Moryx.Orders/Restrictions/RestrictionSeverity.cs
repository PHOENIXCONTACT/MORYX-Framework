// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Orders.Restrictions
{
    /// <summary>
    /// Severity of a rule result
    /// </summary>
    public enum RestrictionSeverity
    {
        /// <summary>
        /// A result which must not be a problem but necessary to inform
        /// </summary>
        Info,

        /// <summary>
        /// A warning about something which should be considered
        /// </summary>
        Warning,

        /// <summary>
        /// A result which is a problem and must be avoided
        /// </summary>
        Error
    }
}