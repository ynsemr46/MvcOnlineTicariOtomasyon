using System;
using System.Collections.Generic;

namespace MvcOnlineTicariOtomasyon.Models.Siniflar
{
    public class SinifGrup
    {
        public string Sehir { get; set; }
        public int Sayi { get; set; }
        public string Kategori { get; set; } // Used
    }

    // For EnBorcluCariler
    public class CariBorcluViewModel
    {
        public int CariId { get; set; }
        public string CariAd { get; set; }
        public string CariSoyad { get; set; }
        public decimal ToplamBorc { get; set; }
    }

    // For SonGiderler
    public class GiderViewModel
    {
        public int GiderId { get; set; }
        public string GiderAciklama { get; set; }
        public decimal Tutar { get; set; }
        public DateTime Tarih { get; set; }
    }

    // For SonTahsilatlar
    public class TahsilatViewModel
    {
        public int TahsilatId { get; set; }
        public decimal TahsilatMiktari { get; set; }
        public DateTime TahsilatTarihi { get; set; }
        public string OdemeTuru { get; set; }
        public string CariAd { get; set; } // Full name (CariAd + CariSoyad)
    }

    // For Partial1 (DepartmanDagilim)
    public class SinifGrup2
    {
        public string Departman { get; set; }
        public int Sayi { get; set; }
    }

    // For Partial4 (MarkaDagilim)
    public class SinifGrup3
    {
        public string marka { get; set; }
        public int sayi { get; set; }
        public int toplamStok { get; set; }
    }

    // ViewModel for the entire KolayTablolar view
    public class HizliBakisViewModel
    {
        public IEnumerable<SinifGrup> SehirDagilim { get; set; }
        public IEnumerable<SinifGrup> KategoriDagilim { get; set; }
        public IEnumerable<CariBorcluViewModel> EnBorcluCariler { get; set; }
        public IEnumerable<GiderViewModel> SonGiderler { get; set; }
        public IEnumerable<TahsilatViewModel> SonTahsilatlar { get; set; }
    }
}