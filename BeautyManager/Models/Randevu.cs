namespace BeautyManager.Models
{
    public class Randevu
    {
        public int Id { get; set; }
        public int MusteriId { get; set; }
        public Musteri? Musteri { get; set; }
        public int PersonelId { get; set; }
        public Personel? Personel { get; set; }
        public DateTime RandevuTarihi { get; set; }
        public string IslemAciklamasi { get; set; } = "";
        public string Durum { get; set; } = "Bekliyor";
        public decimal Ucret { get; set; }
        public bool OdendiMi { get; set; } = false;
        public string Notlar { get; set; } = "";
    }
}