namespace A_Gde_Si_Ti_Pub.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class korisnikObaveznoIme : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Korisniks", "Ime", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Korisniks", "Ime", c => c.String(nullable: false));
        }
    }
}
