namespace MvcOnlineTicariOtomasyon.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class RemoveUrunVaryantAndVaryantOzellikTables : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UrunVaryants", "UrunId", "dbo.Uruns");
            DropForeignKey("dbo.VaryantOzelliks", "VaryantId", "dbo.UrunVaryants");
            DropIndex("dbo.UrunVaryants", new[] { "UrunId" });
            DropIndex("dbo.VaryantOzelliks", new[] { "VaryantId" });
            DropTable("dbo.UrunVaryants");
            DropTable("dbo.VaryantOzelliks");
        }

        public override void Down()
        {
            CreateTable(
                "dbo.VaryantOzelliks",
                c => new
                {
                    VaryantOzellikId = c.Int(nullable: false, identity: true),
                    VaryantId = c.Int(nullable: false),
                    OzellikAdi = c.String(maxLength: 50, unicode: false),
                    OzellikDegeri = c.String(maxLength: 100, unicode: false),
                })
                .PrimaryKey(t => t.VaryantOzellikId);

            CreateTable(
                "dbo.UrunVaryants",
                c => new
                {
                    VaryantId = c.Int(nullable: false, identity: true),
                    UrunId = c.Int(nullable: false),
                    VaryantAdi = c.String(maxLength: 250, unicode: false),
                    Stok = c.Short(nullable: false),
                    FiyatFarki = c.Decimal(nullable: false, precision: 18, scale: 2),
                    VaryantGorsel = c.String(),
                })
                .PrimaryKey(t => t.VaryantId);

            CreateIndex("dbo.VaryantOzelliks", "VaryantId");
            CreateIndex("dbo.UrunVaryants", "UrunId");
            AddForeignKey("dbo.VaryantOzelliks", "VaryantId", "dbo.UrunVaryants", "VaryantId", cascadeDelete: true);
            AddForeignKey("dbo.UrunVaryants", "UrunId", "dbo.Uruns", "UrunId", cascadeDelete: true);
        }
    }
}
