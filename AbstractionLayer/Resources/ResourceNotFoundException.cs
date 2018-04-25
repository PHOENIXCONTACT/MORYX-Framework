using System.Runtime.Serialization;
using Marvin.AbstractionLayer.Capabilities;

namespace Marvin.AbstractionLayer.Resources
{
    /// <summary>
    /// Exception which can be thrown iff a resource was not found
    /// </summary>
    public class ResourceNotFoundException : MarvinException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ResourceNotFoundException()
        {
        }

        /// <summary>
        /// Initializes a new instance with serialized data.
        /// </summary>
        /// <param name="si">The SerializationInfo that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The StreamingContext that contains contextual information about the source or destination.</param>
        public ResourceNotFoundException(SerializationInfo si, StreamingContext context)
            : base(si, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceNotFoundException"/> class.
        /// </summary>
        /// <param name="requiredCapabilities">The required capabilities.</param>
        public ResourceNotFoundException(ICapabilities requiredCapabilities)
            : base($"No resource found providing capabilities or too many matches: {requiredCapabilities}")
        {
        }

        /// <summary>
        /// Initialize the exception with a database id
        /// </summary>
        /// <param name="id">Id that was not found</param>
        public ResourceNotFoundException(long id)
            : base($"No resource found with id: {id}")
        {
        }
    }
}