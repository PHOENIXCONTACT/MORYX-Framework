// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.ProcessData.Endpoints.Models;

namespace Moryx.ProcessData.Endpoints.Services
{
    public interface IListenerService
    {
        Models.Listener GetListener(string name);
        ListenersResponse GetListeners();
        Models.Listener UpdateListener(string name, Models.Listener listenerDto);
    }
}
