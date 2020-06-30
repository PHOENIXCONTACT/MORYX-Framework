// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Marvin.Bindings
{
    /// <summary>
    /// Extended <see cref="IBindingResolver"/> to manipulate the recursive chain
    /// </summary>
    public interface IBindingResolverChain : IBindingResolver
    {
        /// <summary>
        /// Previous resolver in the chain. This can be used to manipulate the chain during execution
        /// </summary>
        IBindingResolverChain PreviousResolver { get; set; }

        /// <summary>
        /// Next resolver as part of a chain of responsibility
        /// </summary>
        IBindingResolverChain NextResolver { get; set; }
    }
}
