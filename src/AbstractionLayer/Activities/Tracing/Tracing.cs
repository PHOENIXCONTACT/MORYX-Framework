using System;
using System.Linq;
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
        public int Progress { get; set; }

        ///
        // ReSharper disable once InconsistentNaming <-- too cool to rename :P
        public Sparta Transform<Sparta>() where Sparta
            : class, IActivityTracing, new()
        {
            if (this is Sparta)
                return this as Sparta;

            var replacement = new Sparta();
            var replacementType = typeof(Sparta);
            var sharedProperties = GetType().GetProperties()
                .Where(p => p.DeclaringType.IsAssignableFrom(replacementType));

            foreach (var property in sharedProperties)
            {
                var value = property.GetValue(this);
                if (property.CanWrite)
                    property.SetValue(replacement, value);
            }

            return replacement;
        }
    }
}