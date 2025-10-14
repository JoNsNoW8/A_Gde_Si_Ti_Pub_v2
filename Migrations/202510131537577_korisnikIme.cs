namespace A_Gde_Si_Ti_Pub.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class korisnikIme : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Korisniks", "Ime", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Korisniks", "Ime");
        }
    }
}
