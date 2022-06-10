namespace Ica.SalesOrders.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _003 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WsMappingTable", "Sortable", c => c.Boolean(nullable: false));
            AddColumn("dbo.WsMappingTable", "Width", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WsMappingTable", "Width");
            DropColumn("dbo.WsMappingTable", "Sortable");
        }
    }
}
