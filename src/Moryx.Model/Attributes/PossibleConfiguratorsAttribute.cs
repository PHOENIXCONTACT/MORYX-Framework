// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Model.Attributes
{
    /// <summary>
    /// Registration attribute for data model configurators
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PossibleConfiguratorsAttribute : Attribute
    {
    }
}
