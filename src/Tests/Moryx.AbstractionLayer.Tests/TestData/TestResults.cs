// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;

namespace Moryx.AbstractionLayer.Tests.TestData
{
    public enum TestResults
    {
        Success = 1,
        [Display(Name = "This is a failed result")]
        Failed = 2
    }
}

