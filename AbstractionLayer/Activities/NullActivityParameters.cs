using System.Runtime.Serialization;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Class to enforce the null object pattern
    /// </summary>
    [DataContract]
    public sealed class NullActivityParameters : IParameters
    {
        /// <see cref="IParameters"/>
        public IParameters Bind(IProcess process)
        {
            return this;
        }
    }
}