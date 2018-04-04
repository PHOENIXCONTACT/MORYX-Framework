namespace Marvin.TestTools.Test.Inheritance.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
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
                        Discriminator = c.String(maxLength: 128),
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("subschema.SuperCarEntity", "Id", "myschema.CarEntity");
            DropForeignKey("subschema.WheelEntity", "CarId", "myschema.CarEntity");
            DropIndex("subschema.SuperCarEntity", new[] { "Id" });
            DropIndex("subschema.WheelEntity", "IX_Car_Id");
            DropTable("subschema.SuperCarEntity");
            DropTable("anotherschema.Another");
            DropTable("subschema.JsonEntity");
            DropTable("subschema.HugePocoEntity");
            DropTable("subschema.HouseEntity");
            DropTable("subschema.WheelEntity");
            DropTable("myschema.CarEntity");
        }
    }
}
