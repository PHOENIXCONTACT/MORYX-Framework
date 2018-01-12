namespace Marvin.TestTools.Test.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using Marvin.Model.Npgsql;
    using System.Data.Entity.Migrations.Model;
    using System.Data.Entity.Migrations.Infrastructure;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            ((IDbMigration)this).AddOperation(new AddSchemaOperation("myschema"));
            
            ((IDbMigration)this).AddOperation(new AddSchemaOperation("subschema"));
            
            ((IDbMigration)this).AddOperation(new AddSchemaOperation("anotherschema"));
            
            CreateTable(
                "myschema.CarEntity",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Price = c.Int(nullable: false),
                        Image = c.Binary(),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        Deleted = c.DateTime(),
                        Performance = c.Int(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "subschema.WheelEntity",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        WheelType = c.Int(nullable: false),
                        CarId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("myschema.CarEntity", t => t.CarId)
                .Index(t => t.CarId, name: "IX_Car_Id");
            
            CreateTable(
                "subschema.HouseEntity",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Size = c.Int(nullable: false),
                        IsMethLabratory = c.Boolean(nullable: false),
                        IsBurnedDown = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        Deleted = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "subschema.HugePocoEntity",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Float1 = c.Double(nullable: false),
                        Name1 = c.String(),
                        Number1 = c.Int(nullable: false),
                        Float2 = c.Double(nullable: false),
                        Name2 = c.String(),
                        Number2 = c.Int(nullable: false),
                        Float3 = c.Double(nullable: false),
                        Name3 = c.String(),
                        Number3 = c.Int(nullable: false),
                        Float4 = c.Double(nullable: false),
                        Name4 = c.String(),
                        Number4 = c.Int(nullable: false),
                        Float5 = c.Double(nullable: false),
                        Name5 = c.String(),
                        Number5 = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "subschema.JsonEntity",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        JsonData = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "anotherschema.Another",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        Deleted = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("subschema.WheelEntity", "CarId", "myschema.CarEntity");
            DropIndex("subschema.WheelEntity", "IX_Car_Id");
            DropTable("anotherschema.Another");
            DropTable("subschema.JsonEntity");
            DropTable("subschema.HugePocoEntity");
            DropTable("subschema.HouseEntity");
            DropTable("subschema.WheelEntity");
            DropTable("myschema.CarEntity");
            ((IDbMigration)this).AddOperation(new RemoveSchemaOperation("anotherschema"));
            
            ((IDbMigration)this).AddOperation(new RemoveSchemaOperation("subschema"));
            
            ((IDbMigration)this).AddOperation(new RemoveSchemaOperation("myschema"));
            
        }
    }
}
