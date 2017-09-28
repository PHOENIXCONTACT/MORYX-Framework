namespace Marvin.Bindings
{
    /// <summary>
    /// Base class for all <see cref="IBindingResolver"/> that implements the 
    /// chain of responsibilities
    /// </summary>
    public abstract class BindingResolverBase : IBindingResolverChain
    {
        /// <inheritdoc />
        public IBindingResolverChain PreviousResolver { get; set; }

        /// <inheritdoc />
        public IBindingResolverChain NextResolver { get; set; }


        /// <inheritdoc />
        public abstract object Resolve(object source);

        /// <summary>
        /// Proceed with the chain
        /// </summary>
        /// <param name="result">Result of this resolvers execution</param>
        protected object Proceed(object result)
        {
            if (NextResolver == null || result == null)
                return result;

            return NextResolver.Resolve(result);
        }
    }
}