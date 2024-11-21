// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Globalization;

namespace Moryx.Bindings
{
    /// <summary>
    /// Binding resolver that converts objects to formatted strings
    /// </summary>
    public class FormatBindingResolver : BindingResolverBase
    {
        private readonly string _format;

        /// <summary>
        /// Create new instance of the format resolver with a given format
        /// </summary>
        /// <param name="format">Desired format of the string</param>
        public FormatBindingResolver(string format)
        {
            _format = format;
        }

        /// <inheritdoc />
        protected override object Resolve(object source)
        {
            var formattable = source as IFormattable;
            return formattable?.ToString(_format, CultureInfo.CurrentCulture) ?? source.ToString();
        }

        /// <inheritdoc />
        protected sealed override bool Update(object source, object value)
        {
            throw new NotImplementedException();
        }
    }
}
