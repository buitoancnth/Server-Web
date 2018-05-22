namespace ServerBTS2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NhatKies", "TieuDe", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.NhatKies", "TieuDe");
        }
    }
}
