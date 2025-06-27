using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcOnlineTicariOtomasyon.Models.Siniflar
{
    public class KategoriOzellik
    {
        [Key]
        public int KategoriOzellikId { get; set; }

        public int KategoriId { get; set; }
        [ForeignKey("KategoriId")]
        public virtual Kategori Kategori { get; set; }

        [Column(TypeName = "Varchar")]
        [StringLength(50)]
        public string OzellikAdi { get; set; } // "Beden", "Renk", "RAM" gibi
    }
}