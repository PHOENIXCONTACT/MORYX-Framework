// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Marvin.AbstractionLayer.Drivers.Scales
{
    /// <summary>
    /// Interface for weight scales to request a weight
    /// </summary>
    public interface IWeightScaleDriver : IDriver
    {
        /// <summary>
        /// Requests the weight from the scale.
        /// </summary>
        void GetCurrentWeight(DriverResponse<WeightResult> callback);

        /// <summary>
        /// Occurs when the weight scale weight was read.
        /// </summary>
        event EventHandler<WeightResult> WeightRead;
    }
}
