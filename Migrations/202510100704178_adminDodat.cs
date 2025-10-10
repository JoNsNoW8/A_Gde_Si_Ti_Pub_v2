namespace A_Gde_Si_Ti_Pub.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adminDodat : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Ocenas", "Komentar", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Ocenas", "Komentar");
        }
    }
}
