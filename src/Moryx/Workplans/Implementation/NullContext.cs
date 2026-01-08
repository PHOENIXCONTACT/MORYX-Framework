// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans
{
    /// <summary>
    /// Null object pattern for the workplan context
    /// </summary>
    public struct NullContext : IWorkplanContext
    {
        /// <see cref="IWorkplanContext"/>
        public bool IsDisabled(IWorkplanStep step)
        {
            return false;
        }
    }
}
