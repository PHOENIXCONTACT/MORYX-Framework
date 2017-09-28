using System;
using System.Globalization;

namespace Marvin.Bindings
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
        public override object Resolve(object source)
        {
            var formattable = source as IFormattable;
            return formattable?.ToString(_format, CultureInfo.CurrentCulture) ?? source.ToString();
        }
    }
}