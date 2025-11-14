// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Moryx.Model;

namespace Moryx.Notifications.Publisher.Model
{
    public class NotificationTypeEntity : EntityBase
    {
        public virtual string Type { get; set; }

        public virtual int Severity { get; set; }

        public virtual string Identifier { get; set; }

        public virtual string ExtensionData { get; set; }

        public virtual bool IsDisabled { get; set; }
    }
}

