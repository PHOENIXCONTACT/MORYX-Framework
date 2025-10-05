// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Configuration;

namespace Moryx.Notifications.Publisher
{
    /// <summary>
    /// Configuration of the <see cref="ModuleController"/>
    /// </summary>
    [DataContract]
    public class ModuleConfig : ConfigBase
    {
    }
}

