namespace MvcOnlineTicariOtomasyon.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class RemoveUrunOzellikTable : DbMigration
    {
        public override void Up()
        {
            // UrunOzelliks zaten manuel silindiği için bu adımları kaldırıyoruz
            // DropForeignKey("dbo.UrunOzelliks", "UrunId", "dbo.Uruns");
            // DropIndex("dbo.UrunOzelliks", new[] { "UrunId" });
            // DropTable("dbo.UrunOzelliks");

            // Yeni tabloları oluştur
            CreateTable(
                "dbo.UrunVaryants",
                c => new
                {
                    VaryantId = c.Int(nullable: false, identity: true),
                    UrunId = c.Int(nullable: false),
                    VaryantAdi = c.String(maxLength: 250, unicode: false),
                    Stok = c.Short(nullable: false),
                    FiyatFarki = c.Decimal(nullable: false, precision: 18, scale: 2),
                })
                .PrimaryKey(t => t.VaryantId)
                .ForeignKey("dbo.Uruns", t => t.UrunId, cascadeDelete: true)
                .Index(t => t.UrunId);

            CreateTable(
                "dbo.VaryantOzelliks",
                c => new
                {
                    VaryantOzellikId = c.Int(nullable: false, identity: true),
                    VaryantId = c.Int(nullable: false),
                    OzellikAdi = c.String(maxLength: 50, unicode: false),
                    OzellikDegeri = c.String(maxLength: 100, unicode: false),
                })
                .PrimaryKey(t => t.VaryantOzellikId)
                .ForeignKey("dbo.UrunVaryants", t => t.VaryantId, cascadeDelete: true)
                .Index(t => t.VaryantId);
        }

        public override void Down()
        {
            DropForeignKey("dbo.VaryantOzelliks", "VaryantId", "dbo.UrunVaryants");
            DropForeignKey("dbo.UrunVaryants", "UrunId", "dbo.Uruns");
            DropIndex("dbo.VaryantOzelliks", new[] { "VaryantId" });
            DropIndex("dbo.UrunVaryants", new[] { "UrunId" });
            DropTable("dbo.VaryantOzelliks");
            DropTable("dbo.UrunVaryants");

            // Eski tabloyu geri oluştur (isteğe bağlı, manuel silindiği için bunu da kaldırabilirsiniz)
            CreateTable(
                "dbo.UrunOzelliks",
                c => new
                {
                    UrunOzellikId = c.Int(nullable: false, identity: true),
                    UrunId = c.Int(nullable: false),
                    OzellikAdi = c.String(maxLength: 50, unicode: false),
                    OzellikDegeri = c.String(maxLength: 100, unicode: false),
                    Stok = c.Short(nullable: false),
                })
                .PrimaryKey(t => t.UrunOzellikId)
                .ForeignKey("dbo.Uruns", t => t.UrunId, cascadeDelete: true)
                .Index(t => t.UrunId);
        }
    }
}
