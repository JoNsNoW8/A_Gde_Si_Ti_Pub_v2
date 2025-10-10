namespace A_Gde_Si_Ti_Pub.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class indDecimalPromenjeni : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Porudzbinas", "UkupnaCena", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Proizvods", "Cena", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Proizvods", "Cena", c => c.Int(nullable: false));
            AlterColumn("dbo.Porudzbinas", "UkupnaCena", c => c.Int(nullable: false));
        }
    }
}
