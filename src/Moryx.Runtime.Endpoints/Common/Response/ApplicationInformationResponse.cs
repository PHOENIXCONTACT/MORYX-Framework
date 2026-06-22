// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using System.Runtime.Versioning;

namespace Moryx.Runtime.Endpoints.Common.Response;

/// <summary>
/// Response contract for application information
/// </summary>
public class ApplicationInformationResponse
{
    /// <summary>
    /// Product name of the entry assembly, read from <see cref="AssemblyProductAttribute"/>
    /// </summary>
    public string AssemblyProduct { get; set; }

    /// <summary>
    /// Version of the entry assembly, read from <see cref="AssemblyVersionAttribute"/>
    /// </summary>
    public string AssemblyVersion { get; set; }

    /// <summary>
    /// Informational version of the entry assembly, read from <see cref="AssemblyVersionAttribute"/>
    /// </summary>
    public string AssemblyInformationalVersion { get; set; }

    /// <summary>
    /// Description of the entry assembly, read from <see cref="AssemblyDescriptionAttribute"/>
    /// </summary>
    public string AssemblyDescription { get; set; }

    /// <summary>
    /// Company name of the entry assembly, read from <see cref="AssemblyCompanyAttribute"/>
    /// </summary>
    public string AssemblyCompanyName { get; set; }

    /// <summary>
    /// Title of the entry assembly, read from <see cref="AssemblyTitleAttribute"/>
    /// </summary>
    public string AssemblyTitle { get; set; }

    /// <summary>
    /// Build configuration of the entry assembly, read from <see cref="AssemblyConfigurationAttribute"/>
    /// </summary>
    public string AssemblyConfiguration { get; set; }

    /// <summary>
    /// Target framework of the entry assembly, read from <see cref="TargetFrameworkAttribute"/>
    /// </summary>
    public string TargetFramework { get; set; }

    /// <summary>
    /// Copyright information of the entry assembly, read from <see cref="AssemblyCopyrightAttribute"/>
    /// </summary>
    public string AssemblyCopyright { get; set; }

    /// <summary>
    /// Trademark information of the entry assembly, read from <see cref="AssemblyTrademarkAttribute"/>
    /// </summary>
    public string AssemblyTrademark { get; set; }
}
