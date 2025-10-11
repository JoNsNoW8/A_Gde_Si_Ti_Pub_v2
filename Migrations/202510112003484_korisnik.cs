namespace A_Gde_Si_Ti_Pub.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class korisnik : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Korisniks", "Uloga", c => c.String(maxLength: 20));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Korisniks", "Uloga", c => c.String(nullable: false, maxLength: 20));
        }
    }
}
