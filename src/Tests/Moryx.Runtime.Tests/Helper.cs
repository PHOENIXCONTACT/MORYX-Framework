// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Runtime.Serialization;
using Moryx.Configuration;
using Moryx.Runtime.Configuration;

namespace Moryx.Runtime.Tests
{
    public enum TestMode
    {
        BestCase,
        MoryxException,
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
    public class TestConfig : ConfigBase
    {
        [ModuleStrategy(typeof(IStrategy))]
        public StrategyConfig Strategy { get; set; }

        [ModuleStrategy(typeof(IStrategy))]
        public string StrategyName { get; set; }
    }

    public class TestException : Exception
    {
    }
}
