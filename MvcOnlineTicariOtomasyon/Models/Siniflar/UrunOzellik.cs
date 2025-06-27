using System.ComponentModel.DataAnnotations;

namespace MvcOnlineTicariOtomasyon.Models.Siniflar
{
    public class UrunOzellik
    {
        [Key]
        public int UrunOzellikId { get; set; }
        public int UrunId { get; set; }
        public string OzellikAdi { get; set; }
        public string OzellikDegeri { get; set; }
        public virtual Urun Urun { get; set; }
    }
}