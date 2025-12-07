// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Configuration;

namespace Moryx.Resources.Management;

public class MoviesConfig : ConfigBase
{
    [DataMember]
    public string BaseUrl { get; set; }

    [DataMember]
    [Password] // DataAnnotations attribute
    public string ServiceApiKey { get; set; }
}
