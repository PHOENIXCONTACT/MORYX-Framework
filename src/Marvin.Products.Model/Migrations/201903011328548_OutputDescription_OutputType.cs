namespace Marvin.Products.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OutputDescription_OutputType : DbMigration
    {
        public override void Up()
        {
            AddColumn("products.OutputDescriptionEntity", "OutputType", c => c.Int(nullable: false));
            DropColumn("products.OutputDescriptionEntity", "Success");
        }
        
        public override void Down()
        {
            AddColumn("products.OutputDescriptionEntity", "Success", c => c.Boolean(nullable: false));
            DropColumn("products.OutputDescriptionEntity", "OutputType");
        }
    }
}
