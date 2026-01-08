// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Moryx.Model;

namespace Moryx.ControlSystem.ProcessEngine.Model
{
    public class TokenHolderEntity : EntityBase
    {
        public virtual long HolderId { get; set; }

        /// <summary>
        /// Json array of all tokens
        /// </summary>
        public virtual string Tokens { get; set; }

        #region Navigation properties

        public virtual long ProcessId { get; set; }

        public virtual ProcessEntity Process { get; set; }

        #endregion
    }
}

