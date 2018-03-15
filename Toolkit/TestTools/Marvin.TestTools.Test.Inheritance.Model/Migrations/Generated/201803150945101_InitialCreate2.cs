namespace Marvin.TestTools.Test.Inheritance.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using Marvin.Model.Npgsql;
    using System.Data.Entity.Migrations.Model;
    using System.Data.Entity.Migrations.Infrastructure;
    
    public partial class InitialCreate2 : DbMigration
    {
        public override void Up()
        {
            ((IDbMigration)this).AddOperation(new AddSchemaOperation("myschema"));
            
            ((IDbMigration)this).AddOperation(new AddSchemaOperation("subschema"));
            
            ((IDbMigration)this).AddOperation(new AddSchemaOperation("anotherschema"));
            
            CreateTable(
                "subschema.SuperCarEntity",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        IsSuper = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("myschema.CarEntity", t => t.Id)
                .Index(t => t.Id);
            
            AlterColumn("myschema.CarEntity", "Discriminator", c => c.String(maxLength: 128));
            DropColumn("myschema.CarEntity", "IsSuper");
        }
        
        public override void Down()
        {
            AddColumn("myschema.CarEntity", "IsSuper", c => c.Boolean());
            DropForeignKey("subschema.SuperCarEntity", "Id", "myschema.CarEntity");
            DropIndex("subschema.SuperCarEntity", new[] { "Id" });
            AlterColumn("myschema.CarEntity", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            DropTable("subschema.SuperCarEntity");
            ((IDbMigration)this).AddOperation(new RemoveSchemaOperation("anotherschema"));
            
            ((IDbMigration)this).AddOperation(new RemoveSchemaOperation("subschema"));
            
            ((IDbMigration)this).AddOperation(new RemoveSchemaOperation("myschema"));
            
        }
    }
}
