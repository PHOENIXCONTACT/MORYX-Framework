// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Configuration
{
    /// <summary>
    /// Determines if a config parameter is a password field
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PasswordAttribute : Attribute
    {

    }
}
