namespace A_Gde_Si_Ti_Pub.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class finalIzmeneModela : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Ocenas", "ProizvodId", "dbo.Proizvods");
            DropIndex("dbo.Ocenas", new[] { "ProizvodId" });
            AddColumn("dbo.Proizvods", "Opis", c => c.String(maxLength: 500));
            AddColumn("dbo.Ocenas", "Datum", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Korisniks", "Uloga", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("dbo.Proizvods", "Naziv", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Proizvods", "Slika", c => c.String(maxLength: 200));
            AlterColumn("dbo.Ocenas", "ProizvodId", c => c.Int());
            AlterColumn("dbo.Ocenas", "Komentar", c => c.String(maxLength: 500));
            CreateIndex("dbo.Ocenas", "ProizvodId");
            AddForeignKey("dbo.Ocenas", "ProizvodId", "dbo.Proizvods", "ProizvodId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Ocenas", "ProizvodId", "dbo.Proizvods");
            DropIndex("dbo.Ocenas", new[] { "ProizvodId" });
            AlterColumn("dbo.Ocenas", "Komentar", c => c.String());
            AlterColumn("dbo.Ocenas", "ProizvodId", c => c.Int(nullable: false));
            AlterColumn("dbo.Proizvods", "Slika", c => c.String());
            AlterColumn("dbo.Proizvods", "Naziv", c => c.String());
            AlterColumn("dbo.Korisniks", "Uloga", c => c.String(maxLength: 20));
            DropColumn("dbo.Ocenas", "Datum");
            DropColumn("dbo.Proizvods", "Opis");
            CreateIndex("dbo.Ocenas", "ProizvodId");
            AddForeignKey("dbo.Ocenas", "ProizvodId", "dbo.Proizvods", "ProizvodId", cascadeDelete: true);
        }
    }
}
