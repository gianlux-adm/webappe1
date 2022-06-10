namespace Ica.SalesOrders.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _005 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WsMappingTable", "HtmlClass", c => c.String(maxLength: 8000, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WsMappingTable", "HtmlClass");
        }
    }
}
