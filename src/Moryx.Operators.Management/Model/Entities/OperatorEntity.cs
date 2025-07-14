// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Moryx.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Moryx.Operators.Management.Model;

public class OperatorEntity : ModificationTrackedEntityBase
{
    [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public override long Id
    {
        get => base.Id;
        set => base.Id = value;
    }

    public string Identifier { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Pseudonym { get; set; }

    #region Navigation properties

    // ToDo: EF Core 8 provides the option to use lists directly, i.e. it would be possible to only store the list of ids here
    public virtual ICollection<ResourceLinkEntity> AssignedResources { get; set; } = [];

    #endregion
}

