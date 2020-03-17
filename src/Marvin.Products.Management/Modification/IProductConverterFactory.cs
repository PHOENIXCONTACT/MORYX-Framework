// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Container;

namespace Marvin.Products.Management.Modification
{
    [PluginFactory]
    internal interface IProductConverterFactory
    {
        /// <summary>
        /// Create converter instance
        /// </summary>
        /// <returns></returns>
        IProductConverter Create();

        /// <summary>
        /// Destroy instance after usage
        /// </summary>
        /// <param name="instance"></param>
        void Destroy(IProductConverter instance);
    }
}
