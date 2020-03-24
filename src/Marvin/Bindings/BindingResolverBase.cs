// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

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
        object IBindingResolver.Resolve(object source)
        {
            var result = Resolve(source);

            if (NextResolver == null || result == null)
                return result;

            return NextResolver.Resolve(result);
        }

        /// <summary>
        /// Resolve the value of this element in the chain
        /// </summary>
        /// <param name="source"></param>
        protected abstract object Resolve(object source);

        /// <inheritdoc />
        bool IBindingResolver.Update(object source, object value)
        {
            // We can not proceed without a source object
            if (source == null)
                return false;

            // We have reached the end of the chain
            if (NextResolver == null)
                return Update(source, value);

            source = Resolve(source);
            return NextResolver.Update(source, value);
        }

        /// <summary>
        /// Set the value on the souce object
        /// </summary>
        protected abstract bool Update(object source, object value);
    }
}
