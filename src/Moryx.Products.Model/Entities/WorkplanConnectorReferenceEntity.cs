// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model;

namespace Moryx.Products.Model
{
    public class WorkplanConnectorReferenceEntity : EntityBase
    {
        public virtual int Index { get; set; }

        public virtual ConnectorRole Role { get; set; }

        public virtual long? ConnectorId { get; set; }

        public virtual long WorkplanStepId { get; set; }

        public virtual WorkplanConnectorEntity Connector { get; set; }

        public virtual WorkplanStepEntity WorkplanStep { get; set; }
    }
}
