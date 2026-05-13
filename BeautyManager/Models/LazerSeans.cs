namespace BeautyManager.Models
{
    public class LazerSeans
    {
        public int Id { get; set; }
        public int MusteriId { get; set; }
        public Musteri? Musteri { get; set; }
        public string Bolge { get; set; } = "";
        public int ToplamSeans { get; set; }
        public int TamamlananSeans { get; set; } = 0;
        public decimal ToplamUcret { get; set; }
        public decimal OdenenUcret { get; set; } = 0;
        public DateTime BaslangicTarihi { get; set; } = DateTime.Now;
        public DateTime? SonrakiSeansTarihi { get; set; }
        public string Durum { get; set; } = "Devam Ediyor";
    }
}