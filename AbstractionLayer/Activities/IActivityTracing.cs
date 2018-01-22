using System;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Interface for activity trace information
    /// </summary>
    public interface IActivityTracing : IQuickCast
    {
        /// <summary>
        /// The time when this activity was started.
        /// </summary>
        DateTime? Started { get; set; }

        /// <summary>
        /// The time when this activity was finished.
        /// </summary>
        DateTime? Completed { get; set;  }

        /// <summary>
        /// Optional tracing text for errors or information
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// Transform the current tracing type to the value of <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Type the <see cref="IActivityTracing" /> is transformed to</typeparam>
        /// <returns>
        /// Transformed object
        /// </returns>
        T Transform<T>() where T : class, IActivityTracing, new();
    }
}