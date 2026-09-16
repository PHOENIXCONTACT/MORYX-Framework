// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StartProject.Asp;

public class GoodPartModel
{
    [Required]
    public string Container { get; set; }

    [ReadOnly(true)]
    public string Order { get; set; }

    [ReadOnly(true)]
    public string Operation { get; set; }

    [PossiblePart]
    public string Product { get; set; }
}
