namespace OpenshopBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PrintBluetoothAddressFieldOnUserModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DeviceUsers", "PrintBluetoothAddress", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DeviceUsers", "PrintBluetoothAddress");
        }
    }
}
