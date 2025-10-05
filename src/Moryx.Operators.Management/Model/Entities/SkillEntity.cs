// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System.ComponentModel.DataAnnotations;
using Moryx.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace Moryx.Operators.Management;

public class SkillEntity : ModificationTrackedEntityBase
{
    [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public override long Id
    {
        get => base.Id;
        set => base.Id = value;
    }

    [Required]
    public DateOnly ObtainedOn { get; set; }

    [Required]
    public DateOnly Expiration { get; set; }
    
    [Required]
    public string OperatorIdentifier { get; set; }

    #region Navigation properties

    public virtual long SkillTypeId { get; set; }

    public virtual SkillTypeEntity SkillType { get; set; }

    #endregion
}

