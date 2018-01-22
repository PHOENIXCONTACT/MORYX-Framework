using Marvin.Workflows;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Interface for the different generic derived types of <see cref="TaskStep{TActivity,TProcParam,TParam}"/>
    /// </summary>
    public interface ITaskStep<out TParam> : IWorkplanStep
        where TParam : IParameters
    {
        /// <summary>
        /// Parameters of this step. This only offers a getter to use covariance and update the object instead of replacing it
        /// </summary>
        TParam Parameters { get; }
    }
}