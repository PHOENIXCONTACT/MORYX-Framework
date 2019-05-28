using System;
using System.Runtime.Serialization;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Class to enforce the null object pattern
    /// </summary>
    [DataContract]
    [Obsolete("Use the base class Tracing instead")]
    public class DefaultTracing : Tracing, IActivityProgress
    {
        ///
        public override string Type => nameof(DefaultTracing);

        /// <summary>
        /// Relative progress directly equals numeric progress
        /// </summary>
        public virtual double Relative => Progress;
    }
}
