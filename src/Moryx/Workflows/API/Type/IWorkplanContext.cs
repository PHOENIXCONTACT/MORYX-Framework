// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans
{
    /// <summary>
    /// Interface for the context an <see cref="IWorkplanInstance"/> is executed on by an <see cref="IWorkplanEngine"/>
    /// </summary>
    public interface IWorkplanContext
    {
        /// <summary>
        /// Check if a step was disabled
        /// </summary>
        bool IsDisabled(IWorkplanStep step);
    }
}
