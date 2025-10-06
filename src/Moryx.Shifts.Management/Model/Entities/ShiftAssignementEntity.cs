// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.ComponentModel.DataAnnotations;
using Moryx.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace Moryx.Shifts.Management
{
    public class ShiftAssignementEntity : ModificationTrackedEntityBase
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override long Id
        {
            get => base.Id;
            set => base.Id = value;
        }

        [Required]
        public long ResourceId { get; set; }

        [Required]
        public string OperatorIdentifier { get; set; }

        [MaxLength(2048)]
        public string? Note { get; set; }

        [Required]
        public int Priority { get; set; }

        [Required]
        public AssignedDays AssignedDays { get; set; }

        public ShiftState State => Shift.State;

        #region Navigation properties

        public virtual long ShiftId { get; set; }

        public virtual ShiftEntity Shift { get; set; }

        #endregion
    }
}

