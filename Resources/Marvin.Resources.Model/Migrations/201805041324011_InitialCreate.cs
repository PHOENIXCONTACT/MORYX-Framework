namespace Marvin.Resources.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "resources.ResourceEntity",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        LocalIdentifier = c.String(),
                        GlobalIdentifier = c.String(),
                        Description = c.String(),
                        ExtensionData = c.String(),
                        Type = c.String(),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        Deleted = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name)
                .Index(t => t.LocalIdentifier)
                .Index(t => t.GlobalIdentifier);
            
            CreateTable(
                "resources.ResourceRelation",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RelationType = c.Int(nullable: false),
                        SourceName = c.String(),
                        SourceId = c.Long(nullable: false),
                        TargetName = c.String(),
                        TargetId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("resources.ResourceEntity", t => t.TargetId, cascadeDelete: true)
                .ForeignKey("resources.ResourceEntity", t => t.SourceId, cascadeDelete: true)
                .Index(t => t.SourceId)
                .Index(t => t.TargetId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("resources.ResourceRelation", "SourceId", "resources.ResourceEntity");
            DropForeignKey("resources.ResourceRelation", "TargetId", "resources.ResourceEntity");
            DropIndex("resources.ResourceRelation", new[] { "TargetId" });
            DropIndex("resources.ResourceRelation", new[] { "SourceId" });
            DropIndex("resources.ResourceEntity", new[] { "GlobalIdentifier" });
            DropIndex("resources.ResourceEntity", new[] { "LocalIdentifier" });
            DropIndex("resources.ResourceEntity", new[] { "Name" });
            DropTable("resources.ResourceRelation");
            DropTable("resources.ResourceEntity");
        }
    }
}
