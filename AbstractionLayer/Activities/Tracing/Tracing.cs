using System;
using System.Runtime.Serialization;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Activity trace information
    /// </summary>
    [DataContract]
    public abstract class Tracing : IActivityTracing
    {
        ///
        public abstract string Type { get; }

        ///
        public DateTime? Started { get; set; }

        ///
        public DateTime? Completed { get; set; }

        ///
        public string Text { get; set; }

        ///
        // ReSharper disable once InconsistentNaming <-- too cool to rename :P
        public Sparta Transform<Sparta>() where Sparta 
            : class, IActivityTracing, new()
        {
            if (this is Sparta)
                return this as Sparta;

            return Fill(new Sparta
            {
                Started = Started,
                Completed = Completed,
                Text = Text
            });
        }

        /// <summary>
        /// Fill the transformed instance
        /// </summary>
        protected abstract T Fill<T>(T instance)
            where T : IActivityTracing;
    }
}