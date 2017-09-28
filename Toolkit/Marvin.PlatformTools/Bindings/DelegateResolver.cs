using System;

namespace Marvin.Bindings
{
    /// <summary>
    /// Resolve binding by invoking a given expression
    /// </summary>
    public class DelegateResolver : BindingResolverBase
    {
        private readonly Func<object, object> _resolveExpression;

        /// <summary>
        /// Create new <see cref="DelegateResolver"/> with delegate
        /// </summary>
        public DelegateResolver(Func<object, object> resolveExpression)
        {
            _resolveExpression = resolveExpression;
        }

        /// <inheritdoc />
        public sealed override object Resolve(object source)
        {
            source = _resolveExpression(source);
            return Proceed(source);
        }
    }
}