﻿// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moryx.AbstractionLayer.Tests.TestData
{
    public enum TestResults
    {
        Success = 1,
        [Display(Name = "This is a failed result")]
        Failed = 2
    }
}

