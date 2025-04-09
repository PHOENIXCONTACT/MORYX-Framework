// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Asp.Integration
{
    /// <summary>
    /// Attribute to declare a web modules event stream
    /// </summary>
    public class ModuleEventStreamAttribute : Attribute
    {
        /// <summary>
        /// Export event stream with given URL
        /// </summary>
        public ModuleEventStreamAttribute(string eventStreamUrl)
        {
            EventStreamUrl = eventStreamUrl;
        }

        /// <summary>
        /// URL of the event stream
        /// </summary>
        public string EventStreamUrl { get; }
    }
}
