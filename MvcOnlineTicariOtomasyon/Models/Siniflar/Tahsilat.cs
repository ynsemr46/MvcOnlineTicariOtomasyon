using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcOnlineTicariOtomasyon.Models.Siniflar
{
    public class Tahsilat
    {
        [Key]
        public int TahsilatId { get; set; }

        public int CariId { get; set; }
        public virtual Cariler Cariler { get; set; }

        public int? FaturaId { get; set; } // Opsiyonel: Faturaya bağlı tahsilat için
        public virtual Faturalar Faturalar { get; set; }

        [Column(TypeName = "Varchar")]
        [StringLength(50)]
        public string OdemeTuru { get; set; } // Nakit, Kredi Kartı, Havale, vb.

        public decimal TahsilatMiktari { get; set; }
        public DateTime TahsilatTarihi { get; set; }

        [Column(TypeName = "Varchar")]
        [StringLength(200)]
        public string Aciklama { get; set; } // Opsiyonel: Tahsilatla ilgili notlar
    }
}