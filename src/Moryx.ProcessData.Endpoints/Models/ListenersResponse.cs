// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ProcessData.Endpoints.Models
{
    public class ListenersResponse
    {
        public List<Listener> Listeners { get; set; }
    }

    public class Listener
    {
        /// <summary>
        /// Name of the listener
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of measurands assigned to this listener
        /// </summary>
        public List<ListenerMeasurand> Measurands { get; set; }

    }

    /// <summary>
    /// Configuration of a measurand
    /// </summary>
    public class ListenerMeasurand
    {
        /// <summary>
        /// Measurands name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Indicates if measurand is enabled
        /// </summary>
        public bool IsEnabled { get; set; }
    }
}
