namespace A_Gde_Si_Ti_Pub.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixPasswordNotMapped : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Korisniks", "Password");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Korisniks", "Password", c => c.String(nullable: false));
        }
    }
}
