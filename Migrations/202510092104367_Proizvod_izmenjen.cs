namespace A_Gde_Si_Ti_Pub.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Proizvod_izmenjen : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Korisniks", "Password", c => c.String(nullable: false));
            AddColumn("dbo.Proizvods", "Slika", c => c.String());
            AddColumn("dbo.Proizvods", "Status", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Proizvods", "Status");
            DropColumn("dbo.Proizvods", "Slika");
            DropColumn("dbo.Korisniks", "Password");
        }
    }
}
