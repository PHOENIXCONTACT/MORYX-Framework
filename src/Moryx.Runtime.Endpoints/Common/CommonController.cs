// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moryx.Runtime.Endpoints.Common.Response;

namespace Moryx.Runtime.Endpoints.Common
{
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
                AssemblyProduct = startAssembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? "MORYX Application",
                AssemblyVersion = version.ToString(3),
                AssemblyInformationalVersion = version.ToString(3),
                AssemblyDescription = startAssembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description ?? "No Description provided!",
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
}

