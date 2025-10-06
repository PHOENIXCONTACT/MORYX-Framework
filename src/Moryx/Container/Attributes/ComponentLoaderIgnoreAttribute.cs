// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Container
{
    /// <summary>
    /// Attribute to ignore assembly component autoloading
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class ComponentLoaderIgnoreAttribute : Attribute
    {

    }
}
