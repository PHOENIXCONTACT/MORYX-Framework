// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using System.Runtime.Versioning;

namespace Moryx.Launcher;

internal static class AssemblyInfoHelper
{
    private static readonly Assembly _entryAssembly = Assembly.GetEntryAssembly();
    public static string CompanyName =>
        _entryAssembly?.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? "N/A";

    public static string ProductName =>
        _entryAssembly?.GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? "N/A";

    public static string Title =>
        _entryAssembly?.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? "N/A";

    public static string Description =>
        _entryAssembly?.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description ?? "N/A";

    public static string Version =>
        _entryAssembly?.GetName()?.Version?.ToString() ?? "N/A";

    public static string Configuration =>
        _entryAssembly?.GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration ?? "N/A";

    public static string TargetFramework =>
        _entryAssembly?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName ?? "N/A";

    public static string Copyright =>
        _entryAssembly?.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? "N/A";

    public static string Trademark =>
        _entryAssembly?.GetCustomAttribute<AssemblyTrademarkAttribute>()?.Trademark ?? "N/A";
}
