// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Moryx.Model;

namespace Moryx.ControlSystem.ProcessEngine.Model
{

    public class ActivityEntity : IEntity
    {
        private long _id;
        // Not derived from EntityBase, because the Id is code generated, not database generated
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual long Id
        {
            get { return _id; }
            set
            {
                if (_id == value)
                    return;

                _id = value;
                IdChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public virtual long TaskId { get; set; }

        public virtual long ResourceId { get; set; }

        public virtual DateTime? Started { get; set; }

        public virtual DateTime? Completed { get; set; }

        public virtual long? ProcessHolderId { get; set; }

        public virtual string TracingText { get; set; }

        public virtual int Progress { get; set; }

        public virtual string TracingData { get; set; }

        public virtual bool Success { get; set; } = false;

        public virtual long? Result { get; set; }

        #region Navigation properties

        public virtual long ProcessId { get; set; }

        public virtual ProcessEntity Process { get; set; }

        public virtual long? JobId { get; set; }

        public virtual JobEntity Job { get; set; }

        public virtual long? TracingTypeId { get; set; }

        public virtual TracingType TracingType { get; set; }

        #endregion

        public event EventHandler IdChanged;
    }
}

