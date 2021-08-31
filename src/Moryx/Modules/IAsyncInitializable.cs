// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Threading.Tasks;

namespace Moryx.Modules
{
    /// <summary>
    /// Because most parts of Moryx are constructed by a DI-Container we do not have control over the time and context
    /// of object creation. Furthermore all major components must only be referenced using their interface so we can not
    /// access their constructor.
    /// To restore the possibility to initialize a component before it is used this base interface should be used.
    /// Code normally executed on construction is therefor moved to Initialize().
    /// </summary>
    public interface IAsyncInitializable
    {
        /// <summary>
        /// Initialize this component and prepare it for incoming tasks. This must only involve preparation and must not start
        /// any active functionality and/or periodic execution of logic.
        /// </summary>
        Task Initialize();
    }
}
