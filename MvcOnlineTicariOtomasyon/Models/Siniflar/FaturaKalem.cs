using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcOnlineTicariOtomasyon.Models.Siniflar
{
    public class FaturaKalem
    {
        [Key]
        public int FaturaKalemId { get; set; }

        [Column(TypeName = "Varchar")]
        [StringLength(100)]
        public string Aciklama { get; set; }
        public decimal Miktar { get; set; } // Changed to decimal for flexibility
        public string Birim { get; set; } // New field for unit
        public decimal BirimFiyat { get; set; }
        public decimal Tutar { get; set; }
        public int Faturaid { get; set; }
        public int? UrunId { get; set; }
        public int? SatisId { get; set; } // New field to link to SatisHareket
        public virtual Faturalar Faturalar { get; set; }
        public virtual Urun Urun { get; set; }
        public virtual SatisHareket SatisHareket { get; set; }
    }
}
