namespace Ica.SalesOrders.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _008 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MappingColors",
                c => new
                    {
                        ColorCode = c.String(nullable: false, maxLength: 2, unicode: false),
                        ColorDescription = c.String(maxLength: 8000, unicode: false),
                        ColorHex = c.String(maxLength: 10, unicode: false),
                    })
                .PrimaryKey(t => t.ColorCode);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.MappingColors");
        }
    }
}
