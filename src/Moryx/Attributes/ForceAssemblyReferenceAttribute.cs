// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx
{
    /// <inheritdoc />
    /// <summary>
    /// This attribute is used to force an assembly reference
    /// Some cirumstances can lead the compiler to the assumption that some third party dlls don't get copied
    /// https://connect.microsoft.com/VisualStudio/feedback/details/652785/visual-studio-does-not-copy-referenced-assemblies-through-the-reference-hierarchy
    /// Example usage:
    /// <example>
    /// Add the following line into the AssemblyInfo.cs of the assembly which third party assembly did not get copied.
    /// [assembly: ForceAssemblyReference(typeof(AsyncMethodBuilderAttribute))]
    /// AsyncMethodBuilderAttribute is a type within the third party assembly. You can use any type within the assembly.
    /// </example>
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class ForceAssemblyReferenceAttribute : Attribute
    {
        /// <inheritdoc />
        public ForceAssemblyReferenceAttribute(Type forcedType)
        {
            //not sure if these two lines are required since 
            //the type is passed to constructor as parameter, 
            //thus effectively being used
            Action<Type> noop = _ => { };
            noop(forcedType);
        }
    }
}
