using System;
using System.ComponentModel;
using Marvin.Testing;

namespace Marvin.Runtime.Configuration
{
    [OpenCoverIgnore]
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
