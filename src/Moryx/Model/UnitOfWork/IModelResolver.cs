// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Model
{
    /// <summary>
    /// Factory capable of creating any 
    /// </summary>
    public interface IModelResolver
    {
        /// <summary>
        /// Create an open context using the model name
        /// </summary>
        IUnitOfWorkFactory GetByName(string modelName);
    }
}
