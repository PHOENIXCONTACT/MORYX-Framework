using System;

namespace Marvin.Container
{
    /// <summary>
    /// Attribute to ignore assembly component autoloading
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class ComponentLoaderIgnoreAttribute : Attribute
    {

    }
}