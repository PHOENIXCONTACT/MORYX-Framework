// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.Scales
{
    /// <summary>
    /// Interface for weight scales to request a weight
    /// </summary>
    public interface IWeightScaleDriver : IDriver
    {
        /// <summary>
        /// Requests the weight from the scale.
        /// </summary>
        Task<WeightResult> GetCurrentWeight();

        /// <summary>
        /// Occurs when the weight scale weight was read.
        /// </summary>
        event EventHandler<WeightResult> WeightRead;
    }
}
