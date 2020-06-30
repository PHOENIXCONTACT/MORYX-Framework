// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Data.Entity.Migrations;

// ReSharper disable once CheckNamespace
namespace Moryx.Resources.Model.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<Moryx.Resources.Model.ResourcesContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            MigrationsDirectory = @"Model\Migrations";
        }
    }
}
