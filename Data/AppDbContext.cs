using Microsoft.EntityFrameworkCore;
using StockAPI.Entities;

namespace StockAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
      
        public DbSet<Sale> Sales { get; set; }
     
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User: Unique Username
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Product → Category (Many-to-One)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Product: Decimal precision
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            // Purchase: TotalAmount precision
            //modelBuilder.Entity<Purchase>()
            //    .Property(p => p.TotalAmount)
            //    .HasPrecision(18, 2);

            // PurchaseDetail → Purchase (Many-to-One)
            //modelBuilder.Entity<PurchaseDetail>()
            //    .HasOne(pd => pd.Purchase)
            //    .WithMany(p => p.PurchaseDetails)
            //    .HasForeignKey(pd => pd.PurchaseId)
            //    .OnDelete(DeleteBehavior.Cascade);

            // PurchaseDetail → Product (Many-to-One)
            //modelBuilder.Entity<PurchaseDetail>()
            //    .HasOne(pd => pd.Product)
            //    .WithMany(p => p.PurchaseDetails)
            //    .HasForeignKey(pd => pd.ProductId)
            //    .OnDelete(DeleteBehavior.Restrict);

            // PurchaseDetail: UnitPrice precision
            //modelBuilder.Entity<PurchaseDetail>()
            //    .Property(pd => pd.UnitPrice)
            //    .HasPrecision(18, 2);

            // SaleDetail → Sale (Many-to-One)
            //modelBuilder.Entity<Sale>()
            //    .HasOne(sd => sd.Sale)
            //    .WithMany(s => s.SaleDetails)
            //    .HasForeignKey(sd => sd.SaleId)
            //    .OnDelete(DeleteBehavior.Cascade);

            // SaleDetail → Product (Many-to-One)
            modelBuilder.Entity<Sale>()
                .HasOne(sd => sd.Product)
                .WithMany(p => p.Sale)
                .HasForeignKey(sd => sd.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // StockMovement → Product (Many-to-One)

            //modelBuilder.Entity<Purchase>()
            //.Property(p => p.PurchaseDate)
            //.HasConversion(
            //v => v.ToDateTime(TimeOnly.MinValue),
            //v => DateOnly.FromDateTime(v));

            // Configure DateOnly for SaleDate
            //modelBuilder.Entity<Sale>()
            //    .Property(s => s.SaleDate)
            //    .HasConversion(
            //        v => v.ToDateTime(TimeOnly.MinValue),
            //        v => DateOnly.FromDateTime(v));

            //base.OnModelCreating(modelBuilder);
        }
    }
}
