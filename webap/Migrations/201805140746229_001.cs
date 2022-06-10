namespace Ica.SalesOrders.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _001 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Functions",
                c => new
                    {
                        FunctionId = c.Int(nullable: false, identity: true),
                        FatherFunctionId = c.Int(),
                        PageTranslation = c.String(maxLength: 8000, unicode: false),
                        KeyTranslation = c.String(maxLength: 8000, unicode: false),
                        Description = c.String(maxLength: 8000, unicode: false),
                        Icon = c.String(maxLength: 8000, unicode: false),
                        Url = c.String(maxLength: 8000, unicode: false),
                        Order = c.Int(nullable: false),
                        Enabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.FunctionId);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        RoleId = c.Int(nullable: false, identity: true),
                        RoleDescription = c.String(maxLength: 20, unicode: false),
                    })
                .PrimaryKey(t => t.RoleId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        LoginName = c.String(maxLength: 40, unicode: false),
                        Password = c.String(maxLength: 40, unicode: false),
                        CodiceE1 = c.String(maxLength: 256, unicode: false),
                        Name = c.String(maxLength: 80, unicode: false),
                        RoleId = c.Int(nullable: false),
                        Enabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Roles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Languages",
                c => new
                    {
                        LanguageID = c.String(nullable: false, maxLength: 2, unicode: false),
                        LanguageName = c.String(nullable: false, maxLength: 25, unicode: false),
                        LanguageEnabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.LanguageID);
            
            CreateTable(
                "dbo.Translations",
                c => new
                    {
                        IdTranslation = c.Int(nullable: false, identity: true),
                        Page = c.String(maxLength: 40, unicode: false),
                        Key = c.String(maxLength: 40, unicode: false),
                        LanguageID = c.String(nullable: false, maxLength: 2, unicode: false),
                        Value = c.String(maxLength: 8000, unicode: false),
                    })
                .PrimaryKey(t => t.IdTranslation)
                .ForeignKey("dbo.Languages", t => t.LanguageID, cascadeDelete: true)
                .Index(t => t.LanguageID);
            
            CreateTable(
                "dbo.LogActions",
                c => new
                    {
                        LogActionID = c.Short(nullable: false, identity: true),
                        LogActionName = c.String(maxLength: 8000, unicode: false),
                        LogAreaID = c.Short(nullable: false),
                        ItemIdType = c.String(maxLength: 20, unicode: false),
                        SecondItemIdType = c.String(maxLength: 20, unicode: false),
                    })
                .PrimaryKey(t => t.LogActionID);
            
            CreateTable(
                "dbo.LogAreas",
                c => new
                    {
                        LogAreaID = c.Short(nullable: false, identity: true),
                        LogAreaName = c.String(maxLength: 50, unicode: false),
                        FilterName = c.String(maxLength: 50, unicode: false),
                    })
                .PrimaryKey(t => t.LogAreaID);
            
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        LogID = c.Int(nullable: false, identity: true),
                        LogActionID = c.String(maxLength: 10, unicode: false),
                        ItemID = c.String(maxLength: 10, unicode: false),
                        SecondItemID = c.String(maxLength: 10, unicode: false),
                        AuthorID = c.String(maxLength: 128, unicode: false),
                        Message = c.String(maxLength: 8000, unicode: false),
                        Data = c.DateTime(nullable: false),
                        Country = c.String(maxLength: 2, unicode: false),
                    })
                .PrimaryKey(t => t.LogID);
            
            CreateTable(
                "dbo.Functions_Roles",
                c => new
                    {
                        FunctionId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FunctionId, t.RoleId })
                .ForeignKey("dbo.Functions", t => t.FunctionId, cascadeDelete: true)
                .ForeignKey("dbo.Roles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.FunctionId)
                .Index(t => t.RoleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Translations", "LanguageID", "dbo.Languages");
            DropForeignKey("dbo.Functions_Roles", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.Functions_Roles", "FunctionId", "dbo.Functions");
            DropForeignKey("dbo.Users", "RoleId", "dbo.Roles");
            DropIndex("dbo.Functions_Roles", new[] { "RoleId" });
            DropIndex("dbo.Functions_Roles", new[] { "FunctionId" });
            DropIndex("dbo.Translations", new[] { "LanguageID" });
            DropIndex("dbo.Users", new[] { "RoleId" });
            DropTable("dbo.Functions_Roles");
            DropTable("dbo.Logs");
            DropTable("dbo.LogAreas");
            DropTable("dbo.LogActions");
            DropTable("dbo.Translations");
            DropTable("dbo.Languages");
            DropTable("dbo.Users");
            DropTable("dbo.Roles");
            DropTable("dbo.Functions");
        }
    }
}
