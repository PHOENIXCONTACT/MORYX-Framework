// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Mvc;
using Moryx.Configuration;
using Microsoft.AspNetCore.Http;
using Moryx.Analytics.Server;
using Moryx.Analytics.Server.ModuleController;
using Moryx.Analytics.Web.Model;
using Moryx.Asp.Extensions;
using Moryx.Analytics.Web.Properties;

namespace Moryx.Analytics.Web
{
    [ApiController]
    [Route("api/moryx/analytics/")]
    [Produces("application/json")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IConfigManager _configManager;

        public AnalyticsController(IConfigManager configManager)
        {
            _configManager = configManager;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<DashboardInformation[]> GetAllDashboards()
        {
            var config = _configManager.GetConfiguration<ModuleConfig>();
            if (config == null) return NotFound(new MoryxExceptionResponse { Title = Strings.DASHBOARD_NOT_FOUND });
            return config.Dashboards.ToArray();
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult RemoveDashboard(string name, [FromBody] string url)
        {
            var config = _configManager.GetConfiguration<ModuleConfig>();
            var dashboard = config.Dashboards.FirstOrDefault(d => d.Url.Equals(url));
            if (dashboard == null)
                return NotFound(new MoryxExceptionResponse { Title = Strings.DASHBOARD_NOT_FOUND });
            config.Dashboards.Remove(dashboard);
            _configManager.SaveConfiguration(config);
            return Ok();
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult EditDashboard(string name, [FromBody] ChangedDashboardInformation changedDashboard)
        {
            var config = _configManager.GetConfiguration<ModuleConfig>();
            var dashboard = config.Dashboards.FirstOrDefault(d => d.Url.Equals(changedDashboard.OriginalUrl));
            if (dashboard == null)
                return NotFound(new MoryxExceptionResponse { Title = Strings.DASHBOARD_NOT_FOUND });
            dashboard.Url = changedDashboard.ChangedDashboard.Url;
            dashboard.Name = changedDashboard.ChangedDashboard.Name;
            _configManager.SaveConfiguration(config);
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult AddDashboard([FromBody] DashboardInformation newDashboard)
        {
            var config = _configManager.GetConfiguration<ModuleConfig>();
            var dashboard = config.Dashboards.FirstOrDefault(d => d.Url.Equals(newDashboard.Url));
            if (dashboard != null)
                return BadRequest("Dashboard with this URL already exists");

            config.Dashboards.Add(newDashboard);
            _configManager.SaveConfiguration(config);
            return Ok();
        }

    }
}

