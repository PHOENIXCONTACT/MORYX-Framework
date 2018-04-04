using System;

namespace Marvin.Model
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IsUnicodeAttribute : Attribute
    {
        public bool Unicode { get; }

        public IsUnicodeAttribute(bool isUnicode)
        {
            Unicode = isUnicode;
        }
    }
}