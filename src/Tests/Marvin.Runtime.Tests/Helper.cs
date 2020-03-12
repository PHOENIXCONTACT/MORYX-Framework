// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Runtime.Serialization;
using Marvin.Configuration;
using Marvin.Runtime.Configuration;

namespace Marvin.Runtime.Tests
{
    public enum TestMode
    {
        BestCase,
        MarvinException,
        SystemException
    }

    public enum InvokedMethod
    {
        None,
        Initialize,
        Start,
        Stop
    }

    [DataContract]
    public class TestConfig : IConfig
    {
        /// <summary>
        /// Current state of the config object. This should be decorated with the data member in order to save
        /// the valid state after finalized configuration.
        /// </summary>
        [DataMember]
        public ConfigState ConfigState { get; set; }

        /// <summary>
        /// Exception message if load failed. This must not be decorated with a data member attribute.
        /// </summary>
        public string LoadError { get; set; }

        [ModuleStrategy(typeof(IStrategy))]
        public StrategyConfig Strategy { get; set; }

        [ModuleStrategy(typeof(IStrategy))]
        public string StrategyName { get; set; }
    }

    public class TestException : Exception
    {
    }
}
