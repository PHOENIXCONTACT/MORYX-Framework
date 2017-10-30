namespace Marvin.Bindings
{
    /// <summary>
    /// Null resolver that simply returns the source
    /// </summary>
    public class NullResolver : BindingResolverBase
    {
        /// <inheritdoc />
        protected sealed override object Resolve(object source)
        {
            return source;
        }

        /// <inheritdoc />
        protected sealed override bool Update(object source, object value)
        {
            throw new System.NotImplementedException();
        }
    }
}