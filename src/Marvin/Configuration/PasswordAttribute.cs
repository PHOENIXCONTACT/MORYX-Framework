using System;

namespace Marvin.Configuration
{
    /// <summary>
    /// Determines if a config parameter is a password field
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PasswordAttribute : Attribute
    {
        
    }
}