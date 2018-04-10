namespace Marvin.Products.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "products.ArticleEntity",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        State = c.Long(nullable: false),
                        ProductionDate = c.DateTime(nullable: false),
                        Identifier = c.String(),
                        NumberType = c.Int(nullable: false),
                        ProductId = c.Long(nullable: false),
                        ParentId = c.Long(),
                        ExtensionData = c.String(),
                        PartLinkId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("products.ArticleEntity", t => t.ParentId, cascadeDelete: true)
                .ForeignKey("products.PartLink", t => t.PartLinkId)
                .ForeignKey("products.ProductEntity", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.State)
                .Index(t => t.ProductId)
                .Index(t => t.ParentId)
                .Index(t => t.PartLinkId);
            
            CreateTable(
                "products.PartLink",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ParentId = c.Long(nullable: false),
                        ChildId = c.Long(nullable: false),
                        PropertyName = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("products.ProductEntity", t => t.ChildId, cascadeDelete: true)
                .ForeignKey("products.ProductEntity", t => t.ParentId, cascadeDelete: true)
                .Index(t => t.ParentId)
                .Index(t => t.ChildId);
            
            CreateTable(
                "products.ProductEntity",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        MaterialNumber = c.String(),
                        Revision = c.Short(nullable: false),
                        TypeName = c.String(),
                        CurrentVersionId = c.Long(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        Deleted = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("products.ProductProperties", t => t.CurrentVersionId, cascadeDelete: true)
                .Index(t => new { t.MaterialNumber, t.Revision }, name: "MaterialNumber_Revision_Index")
                .Index(t => t.MaterialNumber, name: "Material_Number")
                .Index(t => t.CurrentVersionId);
            
            CreateTable(
                "products.ProductProperties",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        State = c.Int(nullable: false),
                        Name = c.String(),
                        ProductId = c.Long(),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        Deleted = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("products.ProductEntity", t => t.ProductId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "products.ProductDocument",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        FileName = c.String(maxLength: 255),
                        FilesystemPath = c.String(maxLength: 255),
                        File = c.Binary(),
                        Version = c.Int(nullable: false),
                        Hash = c.String(maxLength: 65535),
                        ProductId = c.Long(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        Deleted = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("products.ProductEntity", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId);
            
            CreateTable(
                "products.ProductRecipeEntity",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Revision = c.Int(nullable: false),
                        Classification = c.Int(nullable: false),
                        State = c.Int(nullable: false),
                        WorkplanId = c.Long(nullable: false),
                        ProductId = c.Long(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        Deleted = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("products.WorkplanEntity", t => t.WorkplanId, cascadeDelete: true)
                .ForeignKey("products.ProductEntity", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.WorkplanId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "products.WorkplanEntity",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Version = c.Int(nullable: false),
                        State = c.Int(nullable: false),
                        MaxElementId = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        Deleted = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "products.ConnectorEntity",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ConnectorId = c.Long(nullable: false),
                        Name = c.String(),
                        Classification = c.Int(nullable: false),
                        WorkplanId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("products.WorkplanEntity", t => t.WorkplanId, cascadeDelete: true)
                .Index(t => t.WorkplanId);
            
            CreateTable(
                "products.ConnectorReference",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Index = c.Int(nullable: false),
                        Role = c.Int(nullable: false),
                        ConnectorId = c.Long(),
                        StepId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("products.StepEntity", t => t.StepId, cascadeDelete: true)
                .ForeignKey("products.ConnectorEntity", t => t.ConnectorId)
                .Index(t => t.ConnectorId)
                .Index(t => t.StepId);
            
            CreateTable(
                "products.StepEntity",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        StepId = c.Long(nullable: false),
                        Name = c.String(),
                        Assembly = c.String(),
                        NameSpace = c.String(),
                        Classname = c.String(),
                        Parameters = c.String(),
                        WorkplanId = c.Long(nullable: false),
                        SubWorkplanId = c.Long(),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        Deleted = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("products.WorkplanEntity", t => t.SubWorkplanId)
                .ForeignKey("products.WorkplanEntity", t => t.WorkplanId, cascadeDelete: true)
                .Index(t => t.WorkplanId)
                .Index(t => t.SubWorkplanId);
            
            CreateTable(
                "products.OutputDescriptionEntity",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Index = c.Int(nullable: false),
                        Success = c.Boolean(nullable: false),
                        Name = c.String(),
                        MappingValue = c.Long(nullable: false),
                        StepEntityId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("products.StepEntity", t => t.StepEntityId, cascadeDelete: true)
                .Index(t => t.StepEntityId);
            
            CreateTable(
                "products.WorkplanReference",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ReferenceType = c.Int(nullable: false),
                        SourceId = c.Long(nullable: false),
                        TargetId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("products.WorkplanEntity", t => t.SourceId, cascadeDelete: true)
                .ForeignKey("products.WorkplanEntity", t => t.TargetId, cascadeDelete: true)
                .Index(t => t.SourceId)
                .Index(t => t.TargetId);
            
            CreateTable(
                "products.RevisionHistory",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Comment = c.String(),
                        ProductRevisionId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("products.ProductEntity", t => t.ProductRevisionId, cascadeDelete: true)
                .Index(t => t.ProductRevisionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("products.RevisionHistory", "ProductRevisionId", "products.ProductEntity");
            DropForeignKey("products.ArticleEntity", "ProductId", "products.ProductEntity");
            DropForeignKey("products.ArticleEntity", "PartLinkId", "products.PartLink");
            DropForeignKey("products.ProductRecipeEntity", "ProductId", "products.ProductEntity");
            DropForeignKey("products.WorkplanReference", "TargetId", "products.WorkplanEntity");
            DropForeignKey("products.WorkplanReference", "SourceId", "products.WorkplanEntity");
            DropForeignKey("products.ProductRecipeEntity", "WorkplanId", "products.WorkplanEntity");
            DropForeignKey("products.ConnectorEntity", "WorkplanId", "products.WorkplanEntity");
            DropForeignKey("products.ConnectorReference", "ConnectorId", "products.ConnectorEntity");
            DropForeignKey("products.StepEntity", "WorkplanId", "products.WorkplanEntity");
            DropForeignKey("products.StepEntity", "SubWorkplanId", "products.WorkplanEntity");
            DropForeignKey("products.OutputDescriptionEntity", "StepEntityId", "products.StepEntity");
            DropForeignKey("products.ConnectorReference", "StepId", "products.StepEntity");
            DropForeignKey("products.PartLink", "ParentId", "products.ProductEntity");
            DropForeignKey("products.PartLink", "ChildId", "products.ProductEntity");
            DropForeignKey("products.ProductProperties", "ProductId", "products.ProductEntity");
            DropForeignKey("products.ProductDocument", "ProductId", "products.ProductEntity");
            DropForeignKey("products.ProductEntity", "CurrentVersionId", "products.ProductProperties");
            DropForeignKey("products.ArticleEntity", "ParentId", "products.ArticleEntity");
            DropIndex("products.RevisionHistory", new[] { "ProductRevisionId" });
            DropIndex("products.WorkplanReference", new[] { "TargetId" });
            DropIndex("products.WorkplanReference", new[] { "SourceId" });
            DropIndex("products.OutputDescriptionEntity", new[] { "StepEntityId" });
            DropIndex("products.StepEntity", new[] { "SubWorkplanId" });
            DropIndex("products.StepEntity", new[] { "WorkplanId" });
            DropIndex("products.ConnectorReference", new[] { "StepId" });
            DropIndex("products.ConnectorReference", new[] { "ConnectorId" });
            DropIndex("products.ConnectorEntity", new[] { "WorkplanId" });
            DropIndex("products.ProductRecipeEntity", new[] { "ProductId" });
            DropIndex("products.ProductRecipeEntity", new[] { "WorkplanId" });
            DropIndex("products.ProductDocument", new[] { "ProductId" });
            DropIndex("products.ProductProperties", new[] { "ProductId" });
            DropIndex("products.ProductEntity", new[] { "CurrentVersionId" });
            DropIndex("products.ProductEntity", "Material_Number");
            DropIndex("products.ProductEntity", "MaterialNumber_Revision_Index");
            DropIndex("products.PartLink", new[] { "ChildId" });
            DropIndex("products.PartLink", new[] { "ParentId" });
            DropIndex("products.ArticleEntity", new[] { "PartLinkId" });
            DropIndex("products.ArticleEntity", new[] { "ParentId" });
            DropIndex("products.ArticleEntity", new[] { "ProductId" });
            DropIndex("products.ArticleEntity", new[] { "State" });
            DropTable("products.RevisionHistory");
            DropTable("products.WorkplanReference");
            DropTable("products.OutputDescriptionEntity");
            DropTable("products.StepEntity");
            DropTable("products.ConnectorReference");
            DropTable("products.ConnectorEntity");
            DropTable("products.WorkplanEntity");
            DropTable("products.ProductRecipeEntity");
            DropTable("products.ProductDocument");
            DropTable("products.ProductProperties");
            DropTable("products.ProductEntity");
            DropTable("products.PartLink");
            DropTable("products.ArticleEntity");
        }
    }
}
