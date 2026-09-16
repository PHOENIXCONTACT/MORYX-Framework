// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StartProject.Asp;

public class PickPartModel
{
    [Required]
    [Display(Name = "Container Identity")]
    public string Container { get; set; }

    [ReadOnly(true)]
    public string Order { get; set; }

    [ReadOnly(true)]
    public string Operation { get; set; }

    [ReadOnly(true)]
    public string Product { get; set; }
}
