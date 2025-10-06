// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Workplans;

namespace Moryx.AbstractionLayer
{
    /// <summary>
    /// Interface for the different generic derived types of <see cref="TaskStep{TActivity,TProcParam,TParam}"/>
    /// </summary>
    public interface ITaskStep<out TParam> : IWorkplanStep
        where TParam : IParameters
    {
        /// <summary>
        /// Parameters of this step. This only offers a getter to use covariance and update the object instead of replacing it
        /// </summary>
        TParam Parameters { get; }
    }
}
