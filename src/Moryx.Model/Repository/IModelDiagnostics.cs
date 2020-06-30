// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Marvin.Model
{
    /// <summary>
    /// Interface for UnitOfWork diagnostics
    /// </summary>
    public interface IModelDiagnostics
    {
        /// <summary>
        /// Will be executed if the database logs
        /// </summary>
        Action<string> Log { get; set; }
    }
}
