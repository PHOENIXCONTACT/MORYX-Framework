using System;

namespace Marvin.Testing
{
    /// <summary>
    /// Used to mark classes, constructors and methods to be ignored by OpenCover statistics.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property)]
    public class OpenCoverIgnoreAttribute : Attribute
    {
         
    }
}