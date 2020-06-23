namespace Moryx.TestTools.Test.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "cars.CarEntity",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Price = c.Int(nullable: false),
                        Image = c.Binary(),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        Deleted = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "cars.WheelEntity",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        WheelType = c.Int(nullable: false),
                        CarId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("cars.CarEntity", t => t.CarId)
                .Index(t => t.CarId, name: "IX_Car_Id");
            
            CreateTable(
                "testmodel.HouseEntity",
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
                "testmodel.HugePocoEntity",
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
                "testmodel.JsonEntity",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        JsonData = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "testmodel.AnotherEntity",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        Deleted = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "cars.SportCarEntity",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        Performance = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("cars.CarEntity", t => t.Id)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("cars.SportCarEntity", "Id", "cars.CarEntity");
            DropForeignKey("cars.WheelEntity", "CarId", "cars.CarEntity");
            DropIndex("cars.SportCarEntity", new[] { "Id" });
            DropIndex("cars.WheelEntity", "IX_Car_Id");
            DropTable("cars.SportCarEntity");
            DropTable("testmodel.AnotherEntity");
            DropTable("testmodel.JsonEntity");
            DropTable("testmodel.HugePocoEntity");
            DropTable("testmodel.HouseEntity");
            DropTable("cars.WheelEntity");
            DropTable("cars.CarEntity");
        }
    }
}
