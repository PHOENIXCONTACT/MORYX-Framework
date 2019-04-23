using System;
using System.Collections;
using System.IO;

namespace Marvin.Serialization
{
    /// <summary>
    /// Helper class to converts types to enum and vice versa
    /// </summary>
    public static partial class EntryConvert
    {
        /// <summary>
        /// Transform type of entry
        /// </summary>
        /// <returns>Enum representation of the property type</returns>
        public static EntryValueType TransformType(Type propertyType)
        {
			var valueType = EntryValueType.String;
			if (propertyType == typeof(Byte))
			{
				valueType = EntryValueType.Byte;
			}
			else if (propertyType == typeof(Boolean))
			{
				valueType = EntryValueType.Boolean;
			}
			else if (propertyType == typeof(Int16))
			{
				valueType = EntryValueType.Int16;
			}
			else if (propertyType == typeof(UInt16))
			{
				valueType = EntryValueType.UInt16;
			}
			else if (propertyType == typeof(Int32))
			{
				valueType = EntryValueType.Int32;
			}
			else if (propertyType == typeof(UInt32))
			{
				valueType = EntryValueType.UInt32;
			}
			else if (propertyType == typeof(Int64))
			{
				valueType = EntryValueType.Int64;
			}
			else if (propertyType == typeof(UInt64))
			{
				valueType = EntryValueType.UInt64;
			}
			else if (propertyType == typeof(Single))
			{
				valueType = EntryValueType.Single;
			}
			else if (propertyType == typeof(Double))
			{
				valueType = EntryValueType.Double;
			}
            else if (propertyType.IsEnum)
            {
                valueType = EntryValueType.Enum;
            }
			else if (typeof(Stream).IsAssignableFrom(propertyType))
			{
			    valueType = EntryValueType.Stream;
			}
            else if (typeof(IEnumerable).IsAssignableFrom(propertyType) && propertyType != typeof(string))
            {
                valueType = EntryValueType.Collection;
            }
            else if (propertyType.IsClass && propertyType != typeof(string))
            {
                valueType = EntryValueType.Class;
            }
            return valueType;
        }

        /// <summary>
        /// Transform string to typed object based on the type value
        /// </summary>
        /// <param name="type">Desired property type</param>
        /// <param name="value">String value</param>
		/// <param name="formatProvider"><see cref="IFormatProvider"/> used for parsing value</param>
        /// <returns>Typed object with parsed value</returns>
        public static object ToObject(Type type, string value, IFormatProvider formatProvider)
        {
            // Traditional re-transformation
            object result = null;
			if (type == typeof(string))
			{
				result = value;
			}
			else if (type == typeof(Byte))
			{
				result = Byte.Parse(value, formatProvider);
			}
			else if (type == typeof(Boolean))
			{
				result = Boolean.Parse(value);
			}
			else if (type == typeof(Int16))
			{
				result = Int16.Parse(value, formatProvider);
			}
			else if (type == typeof(UInt16))
			{
				result = UInt16.Parse(value, formatProvider);
			}
			else if (type == typeof(Int32))
			{
				result = Int32.Parse(value, formatProvider);
			}
			else if (type == typeof(UInt32))
			{
				result = UInt32.Parse(value, formatProvider);
			}
			else if (type == typeof(Int64))
			{
				result = Int64.Parse(value, formatProvider);
			}
			else if (type == typeof(UInt64))
			{
				result = UInt64.Parse(value, formatProvider);
			}
			else if (type == typeof(Single))
			{
				result = Single.Parse(value, formatProvider);
			}
			else if (type == typeof(Double))
			{
				result = Double.Parse(value, formatProvider);
			}
			else if (type.IsEnum)
            {
                result = Enum.Parse(type, value);
            }

            return result;
        }

		/// <summary>
        /// Transform string to typed object based on the type enum value
        /// </summary>
        /// <param name="type">Entry value type</param>
        /// <param name="value">String value</param>
		/// <param name="formatProvider"><see cref="IFormatProvider"/> used for parsing value</param>
        /// <returns>Typed object with parsed value</returns>
        public static object ToObject(EntryValueType type, string value, IFormatProvider formatProvider)
        {
            // Traditional re-transformation
            object result = null;
			switch (type)
			{
				case EntryValueType.String:
					result = value;
					break;
				case EntryValueType.Byte:
					result = Byte.Parse(value, formatProvider);
					break;
				case EntryValueType.Boolean:
					result = Boolean.Parse(value);
					break;
				case EntryValueType.Int16:
					result = Int16.Parse(value, formatProvider);
					break;
				case EntryValueType.UInt16:
					result = UInt16.Parse(value, formatProvider);
					break;
				case EntryValueType.Int32:
					result = Int32.Parse(value, formatProvider);
					break;
				case EntryValueType.UInt32:
					result = UInt32.Parse(value, formatProvider);
					break;
				case EntryValueType.Int64:
					result = Int64.Parse(value, formatProvider);
					break;
				case EntryValueType.UInt64:
					result = UInt64.Parse(value, formatProvider);
					break;
				case EntryValueType.Single:
					result = Single.Parse(value, formatProvider);
					break;
				case EntryValueType.Double:
					result = Double.Parse(value, formatProvider);
					break;
            }

            return result;
        }
    }
}