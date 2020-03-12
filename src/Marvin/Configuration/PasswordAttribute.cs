// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

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
