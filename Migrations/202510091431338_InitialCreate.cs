namespace A_Gde_Si_Ti_Pub.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DeloviPorudzbines",
                c => new
                    {
                        DeloviPorudzbineID = c.Int(nullable: false, identity: true),
                        PorudzbinaId = c.Int(nullable: false),
                        ProizvodId = c.Int(nullable: false),
                        Kolicina = c.Int(nullable: false),
                        Cena = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.DeloviPorudzbineID)
                .ForeignKey("dbo.Porudzbinas", t => t.PorudzbinaId, cascadeDelete: true)
                .ForeignKey("dbo.Proizvods", t => t.ProizvodId, cascadeDelete: true)
                .Index(t => t.PorudzbinaId)
                .Index(t => t.ProizvodId);
            
            CreateTable(
                "dbo.Porudzbinas",
                c => new
                    {
                        PorudzbinaId = c.Int(nullable: false, identity: true),
                        KorisnikId = c.Int(nullable: false),
                        Datum = c.DateTime(nullable: false),
                        UkupnaCena = c.Int(nullable: false),
                        Status = c.String(),
                    })
                .PrimaryKey(t => t.PorudzbinaId)
                .ForeignKey("dbo.Korisniks", t => t.KorisnikId, cascadeDelete: true)
                .Index(t => t.KorisnikId);
            
            CreateTable(
                "dbo.Korisniks",
                c => new
                    {
                        KorisnikId = c.Int(nullable: false, identity: true),
                        Username = c.String(nullable: false, maxLength: 50),
                        PasswordHash = c.String(nullable: false),
                        Uloga = c.String(maxLength: 20),
                        Email = c.String(maxLength: 100),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.KorisnikId);
            
            CreateTable(
                "dbo.Proizvods",
                c => new
                    {
                        ProizvodId = c.Int(nullable: false, identity: true),
                        Naziv = c.String(),
                        Cena = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProizvodId);
            
            CreateTable(
                "dbo.Ocenas",
                c => new
                    {
                        OcenaId = c.Int(nullable: false, identity: true),
                        Vrednost = c.Int(nullable: false),
                        KorisnikId = c.Int(nullable: false),
                        ProizvodId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OcenaId)
                .ForeignKey("dbo.Korisniks", t => t.KorisnikId, cascadeDelete: true)
                .ForeignKey("dbo.Proizvods", t => t.ProizvodId, cascadeDelete: true)
                .Index(t => t.KorisnikId)
                .Index(t => t.ProizvodId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Ocenas", "ProizvodId", "dbo.Proizvods");
            DropForeignKey("dbo.Ocenas", "KorisnikId", "dbo.Korisniks");
            DropForeignKey("dbo.DeloviPorudzbines", "ProizvodId", "dbo.Proizvods");
            DropForeignKey("dbo.Porudzbinas", "KorisnikId", "dbo.Korisniks");
            DropForeignKey("dbo.DeloviPorudzbines", "PorudzbinaId", "dbo.Porudzbinas");
            DropIndex("dbo.Ocenas", new[] { "ProizvodId" });
            DropIndex("dbo.Ocenas", new[] { "KorisnikId" });
            DropIndex("dbo.Porudzbinas", new[] { "KorisnikId" });
            DropIndex("dbo.DeloviPorudzbines", new[] { "ProizvodId" });
            DropIndex("dbo.DeloviPorudzbines", new[] { "PorudzbinaId" });
            DropTable("dbo.Ocenas");
            DropTable("dbo.Proizvods");
            DropTable("dbo.Korisniks");
            DropTable("dbo.Porudzbinas");
            DropTable("dbo.DeloviPorudzbines");
        }
    }
}
