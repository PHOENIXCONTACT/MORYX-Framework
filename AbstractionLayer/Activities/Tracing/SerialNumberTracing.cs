namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Serialnumber tracing to give information bout random state of number
    /// </summary>
    public class SerialNumberTracing : DefaultTracing
    {
        /// <summary>
        /// determine if number result is random
        /// </summary>
        public bool IsRandom { get; set; }
        /// <summary>
        /// the type of tracing
        /// </summary>
        public override string Type => nameof(SerialNumberTracing);
        /// <summary>
        /// Fill up information
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        protected override T Fill<T>(T instance)
        {
            var casted = instance as SerialNumberTracing;
            if (casted != null)
            {
                casted.IsRandom = IsRandom;
            }
            return instance;
        }
    }
}
