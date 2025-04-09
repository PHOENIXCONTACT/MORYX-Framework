// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Bindings
{
    /// <summary>
    /// Resolve binding by invoking a given expression
    /// </summary>
    public class DelegateResolver : BindingResolverBase
    {
        private readonly Func<object, object> _resolveExpression;
        private readonly Action<object, object> _assignmentExpression;

        /// <summary>
        /// Create new <see cref="DelegateResolver"/> with a resolver expression only
        /// </summary>
        public DelegateResolver(Func<object, object> resolveExpression) : this(resolveExpression, null)
        {
        }

        /// <summary>
        /// Create new <see cref="DelegateResolver"/> with delegate for resolution and assignment
        /// </summary>
        public DelegateResolver(Func<object, object> resolveExpression, Action<object, object> assignmentExpression)
        {
            _resolveExpression = resolveExpression;
            _assignmentExpression = assignmentExpression;
        }

        /// <inheritdoc />
        protected sealed override object Resolve(object source)
        {
            return _resolveExpression(source);
        }

        /// <inheritdoc />
        protected sealed override bool Update(object source, object value)
        {
            if (_assignmentExpression == null)
                return false;

            _assignmentExpression(source, value);
            return true;
        }
    }
}
