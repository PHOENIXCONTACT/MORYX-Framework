// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations.Schema;

namespace Moryx.TestTools.Test.Model
{
    [Table(nameof(SportCarEntity), Schema = TestModelConstants.CarsSchema)]
    public class SportCarEntity : CarEntity
    {
        public virtual int Performance { get; set; }
    }
}
