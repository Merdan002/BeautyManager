namespace BeautyManager.Models
{
    public class MalzemeStok
    {
        public int Id { get; set; }
        public string UrunAdi { get; set; } = "";
        public string Kategori { get; set; } = "";
        public int Miktar { get; set; }
        public string Birim { get; set; } = "Adet";
        public int MinimumStok { get; set; } = 5;
        public decimal BirimFiyat { get; set; }
        public DateTime SonGuncelleme { get; set; } = DateTime.Now;
    }
}