namespace Moryx.TestTools.Test.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RentableHouse : DbMigration
    {
        public override void Up()
        {
            AddColumn("testmodel.HouseEntity", "ToRent", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("testmodel.HouseEntity", "ToRent");
        }
    }
}
