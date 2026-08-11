// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Globalization;
using System.Reflection;
using System.Runtime.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<ApplicationInformationResponse> GetApplicationInfo()
    {
        var entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly is null)
            return BadRequest("Entry assembly could not be determined.");

        return new ApplicationInformationResponse
        {
            AssemblyVersion = entryAssembly.GetName().Version?.ToString() ?? "N/A",
            AssemblyInformationalVersion = entryAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "N/A",
            AssemblyProduct = entryAssembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? "N/A",
            AssemblyDescription = entryAssembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description ?? "N/A",
            AssemblyConfiguration = entryAssembly.GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration ?? "N/A",
            TargetFramework = entryAssembly.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName ?? "N/A",
            AssemblyCopyright = entryAssembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? "N/A",
            AssemblyTrademark = entryAssembly.GetCustomAttribute<AssemblyTrademarkAttribute>()?.Trademark ?? "N/A",
            AssemblyTitle = entryAssembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? "N/A",
            AssemblyCompanyName = entryAssembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? "N/A"
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
