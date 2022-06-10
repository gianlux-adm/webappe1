namespace Ica.SalesOrders.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _006 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WsMappingTable", "PageTranslation", c => c.String(maxLength: 8000, unicode: false));
            AddColumn("dbo.WsMappingTable", "KeyTranslation", c => c.String(maxLength: 8000, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WsMappingTable", "KeyTranslation");
            DropColumn("dbo.WsMappingTable", "PageTranslation");
        }
    }
}
