namespace Marvin.AbstractionLayer.Drivers.Scales
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
            return string.Format("{0:0.00} {1}", Weight, Unit);
        }
    }
}