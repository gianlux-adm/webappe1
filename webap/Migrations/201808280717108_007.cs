namespace Ica.SalesOrders.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _007 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Origins",
                c => new
                    {
                        IdOrigin = c.Int(nullable: false, identity: true),
                        OriginDescription = c.String(maxLength: 8000, unicode: false),
                    })
                .PrimaryKey(t => t.IdOrigin);
            
            CreateTable(
                "dbo.UsersOrigins",
                c => new
                    {
                        IdUserOrigin = c.Int(nullable: false, identity: true),
                        IdUser = c.Int(nullable: false),
                        IdOrigin = c.Int(nullable: false),
                        Username = c.String(maxLength: 100, unicode: false),
                        Password = c.String(maxLength: 100, unicode: false),
                        Token = c.String(maxLength: 8000, unicode: false),
                        TokenCreationDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdUserOrigin)
                .ForeignKey("dbo.Origins", t => t.IdOrigin, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.IdUser, cascadeDelete: true)
                .Index(t => t.IdUser)
                .Index(t => t.IdOrigin);
            
            AddColumn("dbo.Functions", "IdOrigin", c => c.Int());
            AddColumn("dbo.Functions", "Data1", c => c.String(maxLength: 100, unicode: false));
            AddColumn("dbo.Functions", "Data2", c => c.String(maxLength: 100, unicode: false));
            AddColumn("dbo.Functions", "Data3", c => c.String(maxLength: 100, unicode: false));
            CreateIndex("dbo.Functions", "IdOrigin");
            AddForeignKey("dbo.Functions", "IdOrigin", "dbo.Origins", "IdOrigin");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UsersOrigins", "IdUser", "dbo.Users");
            DropForeignKey("dbo.UsersOrigins", "IdOrigin", "dbo.Origins");
            DropForeignKey("dbo.Functions", "IdOrigin", "dbo.Origins");
            DropIndex("dbo.UsersOrigins", new[] { "IdOrigin" });
            DropIndex("dbo.UsersOrigins", new[] { "IdUser" });
            DropIndex("dbo.Functions", new[] { "IdOrigin" });
            DropColumn("dbo.Functions", "Data3");
            DropColumn("dbo.Functions", "Data2");
            DropColumn("dbo.Functions", "Data1");
            DropColumn("dbo.Functions", "IdOrigin");
            DropTable("dbo.UsersOrigins");
            DropTable("dbo.Origins");
        }
    }
}
