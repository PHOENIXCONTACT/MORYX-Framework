// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Moryx.Model;

namespace Moryx.Shifts.Management.Model
{
    public class ShiftEntity : ModificationTrackedEntityBase
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override long Id
        {
            get => base.Id;
            set => base.Id = value;
        }

        [Required]
        public DateOnly Date { get; set; }

        [Required]
        public ShiftState State { get; set; }

        #region Navigation properties

        public virtual long ShiftTypeId { get; set; }

        public virtual ShiftTypeEntity ShiftType { get; set; }

        #endregion
    }

    public enum ShiftState
    {
        New,
        Released,
        Obsolete
    }
}

