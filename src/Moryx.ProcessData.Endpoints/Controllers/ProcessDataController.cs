// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Mvc;
using Moryx.ProcessData.Endpoints.Services;
using Moryx.ProcessData.Endpoints.Models;
using Moryx.Runtime.Modules;
using Moryx.Configuration;

namespace Moryx.ProcessData.Endpoints.Controllers
{

    [ApiController, Route("api/moryx/process-data"), Produces("application/json")]

    public class ProcessDataController : ControllerBase
    {
        private readonly IConfigurationService _configService;
        private readonly IListenerService _listenerService;

        public ProcessDataController(IModuleManager moduleManager, IConfigManager configManager)
        {
            _configService = new ConfigurationService(moduleManager, configManager);
            _listenerService = new ListenerService(moduleManager, configManager);
        }


        /// <summary>
        /// Get all measurands
        /// </summary>
        /// <returns></returns>
        [HttpGet("measurands")]
        public ActionResult<List<MeasurandResponse>> GetMeasurands()
            => Ok(_configService.GetMeasuarands());


        /// <summary>
        /// Get details for a specific measurand
        /// </summary>
        /// <param name="name">Name of the measurand</param>
        /// <returns></returns>
        [HttpGet("measurands/{name}")]
        public ActionResult<MeasurandResponse> GetMeasurand([FromRoute] string name)
        {
            name = name.Trim();
            var measurand = _configService.GetMeasuarand(name);
            if (measurand == null)
            {
                return MeasurandNotFound(name);
            }
            return Ok(measurand);
        }


        /// <summary>
        /// Get a list of a measurands bindings
        /// </summary>
        /// <param name="name">Name of the measurand</param>
        /// <returns>All bindings, configured for the specified measurand</returns>
        [HttpGet("measurands/{name}/bindings")]
        public ActionResult<ConfiguredBindings> GetMeasurandBindings([FromRoute] string name)
        {
            name = name.Trim();
            var measurand = _configService.GetMeasuarand(name);
            if (measurand == null)
            {
                return MeasurandNotFound(name);
            }

            return Ok(_configService.GetMeasuarandBindings(name));
        }

        /// <summary>
        /// Update measurand bindings
        /// </summary>
        /// <remarks>
        ///     Updates the currently bindings configuration for the given measurand
        ///     by the provided bindings.
        /// </remarks>
        /// <param name="name">Name of the measurand</param>
        /// <param name="bindings">A configuration of bindings</param>
        /// <returns>The updated configuration</returns>
        [HttpPut("measurands/{name}/bindings")]
        public ActionResult<ConfiguredBindings> UpdateMeasurandBindings(
            [FromRoute] string name,
            [FromBody] ConfiguredBindings bindings)
        {
            name = name.Trim();
            var measurand = _configService.GetMeasuarand(name);
            if (measurand == null)
            {
                return MeasurandNotFound(name);
            }

            return Ok(_configService
                .UpdateMeasuarandBindings(name, bindings));
        }

        /// <summary>
        /// Get a list of available bindings
        /// </summary>
        /// <param name="name">Name of the measurand</param>
        /// <returns>All bindings, that are may be configured for the specified measurand</returns>
        [HttpGet("measurands/{name}/available-bindings")]
        public ActionResult<MeasurandBindings> GetAvailableBindings([FromRoute] string name)
        {
            name = name.Trim();
            var measurand = _configService.GetMeasuarand(name);
            if (measurand == null)
            {
                return MeasurandNotFound(name);
            }

            return Ok(_configService.GetAvailableBindings(name));
        }


        /// <summary>
        /// Get configured listener configurations
        /// </summary>        
        /// <remarks>
        /// </remarks>
        /// <param name="name"></param>
        /// <param name="configuration"></param>
        [HttpGet("listeners")]
        public ActionResult<ListenersResponse> GetListeners()
            => Ok(_listenerService.GetListeners());


        /// <summary>
        /// Update listener configuration
        /// </summary>        
        /// <remarks>
        /// Updates the specified listeners configuration and reincarnates the process monitor module.
        /// </remarks>
        /// <param name="name"></param>
        /// <param name="configuration"></param>
        [HttpPut("listeners/{name}")]
        public ActionResult<ListenersResponse> UpdateListenerConfiguration(
            [FromRoute] string name,
            [FromBody] Models.Listener configuration)
        {
            name = name.Trim();
            var listener = _listenerService.GetListener(name);
            if (listener == null)
            {
                return ListenerNotFound(name);
            }

            return Ok(_listenerService.UpdateListener(name, configuration));
        }


        private ActionResult MeasurandNotFound(string name)
            => NotFound($"Measurand with name \"{name}\" not found");

        private ActionResult ListenerNotFound(string name)
            => NotFound($"Listener with name \"{name}\" not found");
    }
}


