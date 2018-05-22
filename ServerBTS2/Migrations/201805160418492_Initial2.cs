namespace ServerBTS2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NhatKies", "Loai", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.NhatKies", "Loai");
        }
    }
}
