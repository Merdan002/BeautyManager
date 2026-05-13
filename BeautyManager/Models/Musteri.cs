namespace BeautyManager.Models
{
    public class Musteri
    {
        public int Id { get; set; }
        public string Ad { get; set; } = "";
        public string Soyad { get; set; } = "";
        public string Telefon { get; set; } = "";
        public string Email { get; set; } = "";
        public DateTime KayitTarihi { get; set; } = DateTime.Now;
        public string Notlar { get; set; } = "";
        public ICollection<Randevu> Randevular { get; set; } = new List<Randevu>();
        public ICollection<LazerSeans> LazerSeanslar { get; set; } = new List<LazerSeans>();
    }
}