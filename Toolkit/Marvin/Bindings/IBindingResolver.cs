namespace Marvin.Bindings
{
    /// <summary>
    /// Interface for components that can resolve a single property reference. This is implemented as a double linked list.
    /// </summary>
    public interface IBindingResolver
    {
        /// <summary>
        /// Resolve property from source object
        /// </summary>
        object Resolve(object source);
    }
}