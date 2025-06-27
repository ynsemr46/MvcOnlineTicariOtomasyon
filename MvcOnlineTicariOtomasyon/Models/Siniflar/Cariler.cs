using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcOnlineTicariOtomasyon.Models.Siniflar
{
    public class Cariler
    {
        [Key]
        public int CariId { get; set; }

        [Column(TypeName = "Varchar")]
        [StringLength(30, ErrorMessage = "En fazla 30 karakter yazabilirsiniz")]
        public string CariAd { get; set; }

        [Column(TypeName = "Varchar")]
        [StringLength(30)]
        public string CariSoyad { get; set; }

        [Column(TypeName = "Varchar")]
        [StringLength(14)]
        public string CariSehir { get; set; }

        [Column(TypeName = "Varchar")]
        [StringLength(50)]
        public string CariMail { get; set; }

        [Column(TypeName = "Varchar")]
        [StringLength(20)]
        public string CariSifre { get; set; }

        public bool Durum { get; set; }

        public decimal ToplamBorc { get; set; } // Yeni alan: Cari'nin toplam borcu

        public ICollection<SatisHareket> SatisHarekets { get; set; }
        public virtual ICollection<Tahsilat> Tahsilats { get; set; } // Yeni ilişki: Tahsilatlar
    }
}