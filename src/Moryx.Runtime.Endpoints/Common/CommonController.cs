// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Globalization;
using System.Reflection;
using System.Runtime.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moryx.Runtime.Endpoints.Common.Response;

namespace Moryx.Runtime.Endpoints.Common;

[ApiController]
[Route("common")]
[Produces("application/json")]
public class CommonController : ControllerBase
{
    public CommonController()
    {
    }

    [HttpGet]
    [Route("time")]
    [Authorize(Policy = RuntimePermissions.CanGetGeneralInformation)]
    public ActionResult<ServerTimeResponse> GetServerTime()
    {
        return new ServerTimeResponse
        {
            ServerTime = DateTime.Now.ToString("s", CultureInfo.InvariantCulture)
        };
    }

    [HttpGet("info/application")]
    [Authorize(Policy = RuntimePermissions.CanGetGeneralInformation)]
    public ActionResult<ApplicationInformationResponse> GetApplicationInfo()
    {
        var startAssembly = Assembly.GetEntryAssembly();
        var version = new Version(startAssembly.GetCustomAttribute<AssemblyVersionAttribute>()?.Version ?? "1.0.0");
        return new ApplicationInformationResponse
        {
            AssemblyProduct = startAssembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? "N/A",
            AssemblyVersion = version.ToString(3),
            AssemblyInformationalVersion = version.ToString(3),
            AssemblyDescription = startAssembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description ?? "N/A",
            AssemblyConfiguration =  startAssembly?.GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration ?? "N/A",
            TargetFramework =  startAssembly?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName ?? "N/A",
            AssemblyCopyright =  startAssembly?.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? "N/A",
            AssemblyTrademark =  startAssembly?.GetCustomAttribute<AssemblyTrademarkAttribute>()?.Trademark ?? "N/A",
            AssemblyTitle = startAssembly?.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? "N/A",
            AssemblyCompanyName = startAssembly?.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? "N/A"
        };
    }

    [HttpGet("info/system")]
    [Authorize(Policy = RuntimePermissions.CanGetGeneralInformation)]
    public ActionResult<HostInformationResponse> GetHostInfo()
    {
        return new HostInformationResponse
        {
            MachineName = Environment.MachineName,
            OSInformation = Environment.OSVersion.ToString(),
            UpTime = Environment.TickCount
        };
    }
}
