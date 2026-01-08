// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Bindings
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
