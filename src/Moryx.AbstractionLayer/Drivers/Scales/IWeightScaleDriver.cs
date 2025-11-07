// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers.InOut;

namespace Moryx.AbstractionLayer.Drivers.Scales
{
    /// <summary>
    /// Interface for weight scales to request a weight
    /// </summary>
    public interface IWeightScaleDriver : IInputDriver<MeasuredWeight>
    {

    }
}
