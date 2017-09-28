namespace Marvin.Model
{
    /// <summary>
    /// Interface implemented by all entities to provide self storing property for merged instances
    /// </summary>
    public interface IMergeParent
    {
         /// <summary>
         /// Reference to the merged child
         /// </summary>
         object Child { get; set; }
    }
}