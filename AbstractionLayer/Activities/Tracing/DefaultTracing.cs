using System.Runtime.Serialization;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Class to enforce the null object pattern
    /// </summary>
    [DataContract]
    public class DefaultTracing : Tracing
    {
        ///
        public override string Type => nameof(DefaultTracing);

        ///
        protected override T Fill<T>(T instance)
        {
            return instance;
        }
    }
}
