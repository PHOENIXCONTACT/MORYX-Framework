using System;
using System.ComponentModel;

namespace Marvin.Runtime.Configuration
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CpuCountAttribute : DefaultValueAttribute
    {
        public CpuCountAttribute() : this(0)
        {
        }

        public CpuCountAttribute(int reserve) : base(Environment.ProcessorCount > reserve ? Environment.ProcessorCount - reserve : 1)
        {
        }
    }
}
