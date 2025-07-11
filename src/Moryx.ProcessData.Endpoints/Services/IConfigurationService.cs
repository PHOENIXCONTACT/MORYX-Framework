﻿// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;
using Moryx.ProcessData.Endpoints.Models;
using Moryx.Runtime.Modules;
using System.Collections.Generic;

namespace Moryx.ProcessData.Endpoints.Services
{
    public interface IConfigurationService
    {
        MeasurandBindings GetAvailableBindings(string adapterName);
        Dictionary<string, List<string>> GetBindings(IModule module);
        MeasurandResponse GetMeasuarand(string name);
        ConfiguredBindings GetMeasuarandBindings(string name);
        List<MeasurandResponse> GetMeasuarands();
        IServerModule GetModule(string moduleName);
        ConfiguredBindings UpdateMeasuarandBindings(string name, ConfiguredBindings measurandBindings);
    }
}
