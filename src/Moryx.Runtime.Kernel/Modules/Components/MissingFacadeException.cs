// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Runtime.Kernel
{
    internal class MissingFacadeException : Exception
    {
        public string ModuleName { get; set; }
        public string PropName { get; set; }
        public Type FacadeType { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public MissingFacadeException()
        {
        }

        public MissingFacadeException(string moduleName, string propName, Type facadeType)
            : base($"Found no module hosting a facade of type {facadeType.Name} which was expected by {moduleName}.{propName}")
        {
            ModuleName = moduleName;
            PropName = propName;
            FacadeType = facadeType;
        }
    }
}
