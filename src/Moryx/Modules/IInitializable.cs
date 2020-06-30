// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Marvin.Modules
{
    /// <summary>
    /// Because most parts of Marvin are constructed by a DI-Container we do not have control over the time and context 
    /// of object creation. Furthermore all major components must only be referenced using their interface so we can not 
    /// access their constructor.
    /// To restore the possibilty to initialize a component before it is used this base interface should be used.
    /// Code normally executed on construction is therefor moved to Initialize(). 
    /// </summary>
    public interface IInitializable
    {
        /// <summary>
        /// Initialize this component and prepare it for incoming taks. This must only involve preparation and must not start 
        /// any active functionality and/or periodic execution of logic.
        /// </summary>
        void Initialize();
    }
}
