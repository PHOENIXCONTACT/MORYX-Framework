// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.Scales
{
    /// <summary>
    /// Result weight scales
    /// </summary>
    public class WeightResult : TransmissionResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WeightResult"/> class.
        /// </summary>
        public WeightResult(TransmissionError error) : base(error)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeightResult"/> class.
        /// </summary>
        public WeightResult(double weight, string unit) : base()
        {
            Weight = weight;
            Unit = unit;
        }

        /// <summary>
        /// The readed weight
        /// </summary>
        public double Weight { get; private set; }

        /// <summary>
        /// Gets or sets the unit.
        /// </summary>
        public string Unit { get; set; }

        ///
        public override string ToString()
        {
            return $"{Weight:0.00} {Unit}";
        }
    }
}
