using System.Runtime.Serialization;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Class to enforce the null object pattern
    /// </summary>
    [DataContract]
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
