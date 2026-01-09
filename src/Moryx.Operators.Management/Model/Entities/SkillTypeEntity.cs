// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Moryx.Model;

namespace Moryx.Operators.Management.Model;

public class SkillTypeEntity : ModificationTrackedEntityBase
{
    [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public override long Id
    {
        get => base.Id;
        set => base.Id = value;
    }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; }

    [Required]
    public TimeSpan Duration { get; set; }

    public string CapabilitiesType { get; set; }

    public string CapabilitiesData { get; set; }
}

