using System;

namespace Marvin.Model
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class IsUnicodeAttribute : Attribute
    {
        public bool Unicode { get; set; }

        public IsUnicodeAttribute(bool isUnicode)
        {
            Unicode = isUnicode;
        }
    }
}