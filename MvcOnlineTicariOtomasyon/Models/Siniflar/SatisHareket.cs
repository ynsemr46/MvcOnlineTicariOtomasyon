using System;
using System.ComponentModel.DataAnnotations;

namespace MvcOnlineTicariOtomasyon.Models.Siniflar
{
    public class SatisHareket
    {
        [Key]
        public int SatisId { get; set; }
        //ürün
        //cari
        //personel
        public DateTime Tarih { get; set; }
        public decimal Miktar { get; set; } // Changed from Adet to Miktar to support decimal quantities
        public string Birim { get; set; } //
        public decimal Fiyat { get; set; }
        public decimal ToplamTutar { get; set; }

        public int UrunId { get; set; }
        public int CariId { get; set; }
        public int PersonelId { get; set; }
        public int? FaturaId { get; set; } // New field to link to Fatura

        public virtual Urun Urun { get; set; }
        public virtual Cariler Cariler { get; set; }
        public virtual Personel Personel { get; set; }
        public virtual Faturalar Faturalar { get; set; } // Link to Fatura
    }
}