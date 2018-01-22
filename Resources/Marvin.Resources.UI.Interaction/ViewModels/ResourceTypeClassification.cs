namespace Marvin.Resources.UI.Interaction
{
    /// <summary>
    /// Classification of the resource type
    /// </summary>
    public enum ResourceTypeClassification
    {
        /// <summary>
        /// Unset value
        /// </summary>
        Unset = 0,

        /// <summary>
        /// Production resource type
        /// </summary>
        Production = 1,
        
        /// <summary>
        /// Infrastructure resource type
        /// </summary>
        Infrastructure = 2,

        /// <summary>
        /// Logical resource type
        /// </summary>
        Logical = 4,

        /// <summary>
        /// Structure resource type
        /// </summary>
        Structure = 8,

        /// <summary>
        /// Part resource type
        /// </summary>
        Part = 10,
    }
}