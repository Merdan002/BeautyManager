using Microsoft.EntityFrameworkCore;

namespace BeautyManager.Models
{
    public class BeautyContext : DbContext
    {
        public BeautyContext(DbContextOptions<BeautyContext> options) : base(options) { }

        public DbSet<Musteri> Musteriler { get; set; }
        public DbSet<Personel> Personeller { get; set; }
        public DbSet<Randevu> Randevular { get; set; }
        public DbSet<LazerSeans> LazerSeanslar { get; set; }
        public DbSet<MalzemeStok> MalzemeStoklar { get; set; }
        public DbSet<FiyatListesi> FiyatListeleri { get; set; }
        public DbSet<Kullanici> Kullanicilar { get; set; }
    }
}