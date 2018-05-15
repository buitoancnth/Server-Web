namespace ServerBTS2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.HinhAnhTrams", "Ten", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.HinhAnhTrams", "Ten", c => c.String(nullable: false));
        }
    }
}
