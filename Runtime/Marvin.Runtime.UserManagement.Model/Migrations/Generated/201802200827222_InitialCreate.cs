namespace Marvin.Runtime.UserManagement.Model.Migrations
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
            ((IDbMigration)this).AddOperation(new AddSchemaOperation("usrmgmt"));
            
            CreateTable(
                "usrmgmt.ApplicationAccess",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AccessType = c.Int(nullable: false),
                        ApplicationId = c.Long(),
                        UserGroupId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("usrmgmt.Application", t => t.ApplicationId)
                .ForeignKey("usrmgmt.UserGroup", t => t.UserGroupId)
                .Index(t => t.ApplicationId, name: "IX_Application_Id")
                .Index(t => t.UserGroupId, name: "IX_UserGroup_Id");
            
            CreateTable(
                "usrmgmt.Application",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ApplicationName = c.String(maxLength: 128),
                        ShellId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("usrmgmt.Library", t => t.ShellId, cascadeDelete: true)
                .Index(t => t.ApplicationName)
                .Index(t => t.ShellId, name: "IX_Shell_Id");
            
            CreateTable(
                "usrmgmt.Library",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        LibraryName = c.String(maxLength: 128),
                        FullPath = c.String(maxLength: 256),
                        SortIndex = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.LibraryName);
            
            CreateTable(
                "usrmgmt.LibraryAccess",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserGroupId = c.Long(),
                        LibraryId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("usrmgmt.UserGroup", t => t.UserGroupId)
                .ForeignKey("usrmgmt.Library", t => t.LibraryId)
                .Index(t => t.UserGroupId, name: "IX_UserGroup_Id")
                .Index(t => t.LibraryId, name: "IX_Library_Id");
            
            CreateTable(
                "usrmgmt.OperationGroup",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        GroupName = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.GroupName);
            
            CreateTable(
                "usrmgmt.OperationGroupLink",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AccessType = c.Int(nullable: false),
                        OperationId = c.Long(),
                        OperationGroupId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("usrmgmt.Operation", t => t.OperationId)
                .ForeignKey("usrmgmt.OperationGroup", t => t.OperationGroupId)
                .Index(t => t.OperationId, name: "IX_Operation_Id")
                .Index(t => t.OperationGroupId, name: "IX_OperationGroup_Id");
            
            CreateTable(
                "usrmgmt.Operation",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name);
            
            CreateTable(
                "usrmgmt.UserGroup",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        GroupName = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.GroupName);
            
            CreateTable(
                "usrmgmt.OpGr_OpGr",
                c => new
                    {
                        OperationGroupId = c.Long(nullable: false),
                        OperationGroup_Id1 = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.OperationGroupId, t.OperationGroup_Id1 })
                .ForeignKey("usrmgmt.OperationGroup", t => t.OperationGroupId)
                .ForeignKey("usrmgmt.OperationGroup", t => t.OperationGroup_Id1)
                .Index(t => t.OperationGroupId, name: "IX_OperationGroup_Id")
                .Index(t => t.OperationGroup_Id1);
            
            CreateTable(
                "usrmgmt.LibraryAccess_OperationGroup",
                c => new
                    {
                        LibraryAccessId = c.Long(nullable: false),
                        OperationGroupId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.LibraryAccessId, t.OperationGroupId })
                .ForeignKey("usrmgmt.LibraryAccess", t => t.LibraryAccessId, cascadeDelete: true)
                .ForeignKey("usrmgmt.OperationGroup", t => t.OperationGroupId, cascadeDelete: true)
                .Index(t => t.LibraryAccessId, name: "IX_LibraryAccess_Id")
                .Index(t => t.OperationGroupId, name: "IX_OperationGroup_Id");
            
            CreateTable(
                "usrmgmt.Library_Library",
                c => new
                    {
                        LibraryId = c.Long(nullable: false),
                        Library_Id1 = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.LibraryId, t.Library_Id1 })
                .ForeignKey("usrmgmt.Library", t => t.LibraryId)
                .ForeignKey("usrmgmt.Library", t => t.Library_Id1)
                .Index(t => t.LibraryId, name: "IX_Library_Id")
                .Index(t => t.Library_Id1);
            
            CreateTable(
                "usrmgmt.Library_Application",
                c => new
                    {
                        LibraryId = c.Long(nullable: false),
                        ApplicationId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.LibraryId, t.ApplicationId })
                .ForeignKey("usrmgmt.Library", t => t.LibraryId, cascadeDelete: true)
                .ForeignKey("usrmgmt.Application", t => t.ApplicationId, cascadeDelete: true)
                .Index(t => t.LibraryId, name: "IX_Library_Id")
                .Index(t => t.ApplicationId, name: "IX_Application_Id");
            
        }
        
        public override void Down()
        {
            DropForeignKey("usrmgmt.Application", "ShellId", "usrmgmt.Library");
            DropForeignKey("usrmgmt.Library_Application", "ApplicationId", "usrmgmt.Application");
            DropForeignKey("usrmgmt.Library_Application", "LibraryId", "usrmgmt.Library");
            DropForeignKey("usrmgmt.Library_Library", "Library_Id1", "usrmgmt.Library");
            DropForeignKey("usrmgmt.Library_Library", "LibraryId", "usrmgmt.Library");
            DropForeignKey("usrmgmt.LibraryAccess", "LibraryId", "usrmgmt.Library");
            DropForeignKey("usrmgmt.LibraryAccess", "UserGroupId", "usrmgmt.UserGroup");
            DropForeignKey("usrmgmt.ApplicationAccess", "UserGroupId", "usrmgmt.UserGroup");
            DropForeignKey("usrmgmt.LibraryAccess_OperationGroup", "OperationGroupId", "usrmgmt.OperationGroup");
            DropForeignKey("usrmgmt.LibraryAccess_OperationGroup", "LibraryAccessId", "usrmgmt.LibraryAccess");
            DropForeignKey("usrmgmt.OperationGroupLink", "OperationGroupId", "usrmgmt.OperationGroup");
            DropForeignKey("usrmgmt.OperationGroupLink", "OperationId", "usrmgmt.Operation");
            DropForeignKey("usrmgmt.OpGr_OpGr", "OperationGroup_Id1", "usrmgmt.OperationGroup");
            DropForeignKey("usrmgmt.OpGr_OpGr", "OperationGroupId", "usrmgmt.OperationGroup");
            DropForeignKey("usrmgmt.ApplicationAccess", "ApplicationId", "usrmgmt.Application");
            DropIndex("usrmgmt.Library_Application", "IX_Application_Id");
            DropIndex("usrmgmt.Library_Application", "IX_Library_Id");
            DropIndex("usrmgmt.Library_Library", new[] { "Library_Id1" });
            DropIndex("usrmgmt.Library_Library", "IX_Library_Id");
            DropIndex("usrmgmt.LibraryAccess_OperationGroup", "IX_OperationGroup_Id");
            DropIndex("usrmgmt.LibraryAccess_OperationGroup", "IX_LibraryAccess_Id");
            DropIndex("usrmgmt.OpGr_OpGr", new[] { "OperationGroup_Id1" });
            DropIndex("usrmgmt.OpGr_OpGr", "IX_OperationGroup_Id");
            DropIndex("usrmgmt.UserGroup", new[] { "GroupName" });
            DropIndex("usrmgmt.Operation", new[] { "Name" });
            DropIndex("usrmgmt.OperationGroupLink", "IX_OperationGroup_Id");
            DropIndex("usrmgmt.OperationGroupLink", "IX_Operation_Id");
            DropIndex("usrmgmt.OperationGroup", new[] { "GroupName" });
            DropIndex("usrmgmt.LibraryAccess", "IX_Library_Id");
            DropIndex("usrmgmt.LibraryAccess", "IX_UserGroup_Id");
            DropIndex("usrmgmt.Library", new[] { "LibraryName" });
            DropIndex("usrmgmt.Application", "IX_Shell_Id");
            DropIndex("usrmgmt.Application", new[] { "ApplicationName" });
            DropIndex("usrmgmt.ApplicationAccess", "IX_UserGroup_Id");
            DropIndex("usrmgmt.ApplicationAccess", "IX_Application_Id");
            DropTable("usrmgmt.Library_Application");
            DropTable("usrmgmt.Library_Library");
            DropTable("usrmgmt.LibraryAccess_OperationGroup");
            DropTable("usrmgmt.OpGr_OpGr");
            DropTable("usrmgmt.UserGroup");
            DropTable("usrmgmt.Operation");
            DropTable("usrmgmt.OperationGroupLink");
            DropTable("usrmgmt.OperationGroup");
            DropTable("usrmgmt.LibraryAccess");
            DropTable("usrmgmt.Library");
            DropTable("usrmgmt.Application");
            DropTable("usrmgmt.ApplicationAccess");
            ((IDbMigration)this).AddOperation(new RemoveSchemaOperation("usrmgmt"));
            
        }
    }
}
