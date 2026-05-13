namespace BeautyManager.Models
{
    public class Personel
    {
        public int Id { get; set; }
        public string Ad { get; set; } = "";
        public string Soyad { get; set; } = "";
        public string Telefon { get; set; } = "";
        public string Uzmanlik { get; set; } = "";
        public bool Aktif { get; set; } = true;
        public ICollection<Randevu> Randevular { get; set; } = new List<Randevu>();
    }
}