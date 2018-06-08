using System.Data.Entity.Migrations;

// ReSharper disable once CheckNamespace
namespace Marvin.Resources.Model.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<Marvin.Resources.Model.ResourcesContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            MigrationsDirectory = @"Model\Migrations";
        }
    }
}
