// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

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
