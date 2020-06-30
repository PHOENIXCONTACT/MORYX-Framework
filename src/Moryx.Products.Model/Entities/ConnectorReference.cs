// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Model;

namespace Marvin.Products.Model
{
    public class ConnectorReference : EntityBase
    {
        public virtual int Index { get; set; }

        public virtual ConnectorRole Role { get; set; }

        public virtual long? ConnectorId { get; set; }

        public virtual long StepId { get; set; }

        public virtual ConnectorEntity Connector { get; set; }

        public virtual StepEntity Step { get; set; }
    }
}
