using Marvin.Model.Npgsql;

namespace Marvin.TestTools.Test.Model.Migrations
{
    public sealed class Configuration : NpgsqlDbMigrationConfigurationBase<TestModelContext>
    {
        protected override void Seed(Marvin.TestTools.Test.Model.TestModelContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
