using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcOnlineTicariOtomasyon.Models.Siniflar
{
    public class UrunGorsel
    {
        [Key]
        public int UrunGorselId { get; set; }

        public int UrunId { get; set; }
        [ForeignKey("UrunId")]
        public virtual Urun Urun { get; set; }

        [Column(TypeName = "Varchar")]
        [StringLength(250)]
        public string GorselUrl { get; set; }
    }
}