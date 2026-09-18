// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StartProject.Asp;

public class GoodPartModel
{
    [ReadOnly(true)]
    public string Order { get; set; }

    [ReadOnly(true)]
    public string Operation { get; set; }

    [ReadOnly(true)]
    public string Product { get; set; }

    [Required]
    [Display(Name = "Container Identity")]
    public string Container { get; set; }

    [Required]
    [Display(Name = "Amount to advice")]
    public int Amount { get; set; }

    public override string ToString()
    {
        return $"{Order}-{Operation} {Product}";
    }
}
