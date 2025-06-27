﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcOnlineTicariOtomasyon.Models.Siniflar
{
    public class Urun
    {
        [Key]
        public int UrunId { get; set; }



        [Column(TypeName = "Varchar")]
        [StringLength(30)]
        public string UrunAd { get; set; }

        [Column(TypeName = "Varchar")]
        [StringLength(30)]
        public string Marka { get; set; }
        public short Stok { get; set; }
        public decimal AlisFiyat { get; set; }
        public decimal SatisFiyat { get; set; }
        public bool Durum { get; set; }

        [Column(TypeName = "Varchar")]
        [StringLength(20)]
        public string Birim { get; set; }

        [Column(TypeName = "Varchar")]
        [StringLength(250)]
        public string UrunGorsel { get; set; }

        [Column(TypeName = "Varchar")]
        [StringLength(2000)]
        public string Aciklama { get; set; }

        public int Kategoriid { get; set; }
        public virtual Kategori Kategori { get; set; }

        public ICollection<SatisHareket> SatisHarekets { get; set; }
        public virtual ICollection<UrunOzellik> UrunOzelliks { get; set; }
        public virtual ICollection<UrunGorsel> UrunGorsels { get; set; }
    }
}