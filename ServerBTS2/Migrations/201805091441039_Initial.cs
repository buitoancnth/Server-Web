namespace ServerBTS2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MatDiens", "IDTram", c => c.Int(nullable: false));
            DropColumn("dbo.MatDiens", "IDNhaTram");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MatDiens", "IDNhaTram", c => c.Int(nullable: false));
            DropColumn("dbo.MatDiens", "IDTram");
        }
    }
}
