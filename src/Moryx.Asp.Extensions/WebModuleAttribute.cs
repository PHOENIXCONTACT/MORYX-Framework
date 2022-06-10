// Copyright (c) 2022, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Asp.Integration
{
    /// <summary>
    /// Decorator for web modules
    /// </summary>
    public class WebModuleAttribute
    {
        /// <summary>
        /// Unique route of this module
        /// </summary>
        public string Route { get; }

        /// <summary>
        /// Recognizable icon of this module
        /// </summary>
        public string Icon { get; }

        /// <summary>
        /// Export page as web module under given route
        /// </summary>
        public WebModuleAttribute(string route) : this(route, "favorite")
        {
            
        }

        /// <summary>
        /// Export page as web module under given route and icon
        /// </summary>
        public WebModuleAttribute(string route, string icon)
        {
            Route = route;
            Icon = icon;
        }
    }
}