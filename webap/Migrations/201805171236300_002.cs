namespace Ica.SalesOrders.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _002 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WsMappingTable",
                c => new
                    {
                        IdWsMappingTable = c.Int(nullable: false, identity: true),
                        Priority = c.Int(nullable: false),
                        Type = c.String(maxLength: 40, unicode: false),
                        Description = c.String(maxLength: 200, unicode: false),
                        Field = c.String(maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => t.IdWsMappingTable);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.WsMappingTable");
        }
    }
}
