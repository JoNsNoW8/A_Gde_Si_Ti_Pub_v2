namespace A_Gde_Si_Ti_Pub.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Prodavnica_Porudzbine : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Porudzbinas", "Ime", c => c.String(nullable: false, maxLength: 50));
            AddColumn("dbo.Porudzbinas", "Prezime", c => c.String(nullable: false, maxLength: 50));
            AddColumn("dbo.Porudzbinas", "Adresa", c => c.String(nullable: false, maxLength: 200));
            AddColumn("dbo.Porudzbinas", "Email", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Porudzbinas", "Email");
            DropColumn("dbo.Porudzbinas", "Adresa");
            DropColumn("dbo.Porudzbinas", "Prezime");
            DropColumn("dbo.Porudzbinas", "Ime");
        }
    }
}
