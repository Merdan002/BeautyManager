namespace BeautyManager.Models
{
    public class FiyatListesi
    {
        public int Id { get; set; }
        public string IslemAdi { get; set; } = "";
        public string Kategori { get; set; } = "";
        public decimal Fiyat { get; set; }
        public int Sure { get; set; }
        public bool Aktif { get; set; } = true;
    }
}