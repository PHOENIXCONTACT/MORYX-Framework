// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Moryx.Model
{
    /// <summary>
    /// Base class for <see cref="IEntity"/>
    /// </summary>
    public abstract class EntityBase : IEntity
    {
        private long _id;

        /// <inheritdoc />
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual long Id
        {
            get => _id;
            set
            {
                _id = value;
                IdChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <inheritdoc />
        public event EventHandler IdChanged;
    }
}
