namespace MvcOnlineTicariOtomasyon.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Uruns", "Kategori_KategoriId", "dbo.Kategoris");
            DropIndex("dbo.Uruns", new[] { "Kategori_KategoriId" });
            RenameColumn(table: "dbo.Uruns", name: "Kategori_KategoriId", newName: "Kategoriid");
            AlterColumn("dbo.Uruns", "Kategoriid", c => c.Int(nullable: false));
            CreateIndex("dbo.Uruns", "Kategoriid");
            AddForeignKey("dbo.Uruns", "Kategoriid", "dbo.Kategoris", "KategoriId", cascadeDelete: true);
        }

        public override void Down()
        {
            DropForeignKey("dbo.Uruns", "Kategoriid", "dbo.Kategoris");
            DropIndex("dbo.Uruns", new[] { "Kategoriid" });
            AlterColumn("dbo.Uruns", "Kategoriid", c => c.Int());
            RenameColumn(table: "dbo.Uruns", name: "Kategoriid", newName: "Kategori_KategoriId");
            CreateIndex("dbo.Uruns", "Kategori_KategoriId");
            AddForeignKey("dbo.Uruns", "Kategori_KategoriId", "dbo.Kategoris", "KategoriId");
        }
    }
}
