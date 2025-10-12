namespace A_Gde_Si_Ti_Pub.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dupliranjeProizvoda : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Proizvods", "Naziv", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Proizvods", new[] { "Naziv" });
        }
    }
}
