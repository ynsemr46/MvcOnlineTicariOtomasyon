using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcOnlineTicariOtomasyon.Models.Siniflar
{
    public class Personel
    {
        [Key]
        public int PersonelId { get; set; }


        [Column(TypeName = "Varchar")]
        [StringLength(30)]
        public string PersonelAd { get; set; }

        [Column(TypeName = "Varchar")]
        [StringLength(30)]
        public string PersonelSoyad { get; set; }

        [Column(TypeName = "Varchar")]
        [StringLength(250)]
        public string PersonelGorsel { get; set; }

        [Column(TypeName = "Varchar")]
        [StringLength(50)]
        public string KullaniciAd { get; set; } // Kullanıcı adı

        [Column(TypeName = "Varchar")]
        [StringLength(100)]
        public string Sifre { get; set; } // Şifre (üretimde hash'lenmeli)

        [Column(TypeName = "Char")]
        [StringLength(1)]
        public string Yetki { get; set; } // Yetki: 'P' (Personel)

        public ICollection<SatisHareket> SatisHarekets { get; set; }

        public int DepartmanId { get; set; }
        public virtual Departman Departman { get; set; }

    }
}