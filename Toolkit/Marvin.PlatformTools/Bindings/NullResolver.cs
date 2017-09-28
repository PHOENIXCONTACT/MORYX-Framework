namespace Marvin.Bindings
{
    /// <summary>
    /// Null resolver that simply returns the source
    /// </summary>
    public class NullResolver : BindingResolverBase
    {
        /// <inheritdoc />
        public sealed override object Resolve(object source)
        {
            return Proceed(source);
        }
    }
}