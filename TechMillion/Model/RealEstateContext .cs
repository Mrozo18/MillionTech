using Microsoft.EntityFrameworkCore;

namespace TechMillion.Model
{
    public class RealEstateContext : DbContext
    {
        public RealEstateContext(DbContextOptions<RealEstateContext> options) : base(options) { }

        public DbSet<Owner> Owners { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyImage> PropertyImages { get; set; }
        //public DbSet<PropertyFilter> PropertyFilter { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Owner>()
                .HasKey(o => o.IdOwner);

            modelBuilder.Entity<Property>()
                .HasKey(p => p.IdProperty);

            modelBuilder.Entity<PropertyImage>()
                .HasKey(pi => pi.IdPropertyImage);
            
            modelBuilder.Entity<PropertyTrace>()
                .HasKey(pi => pi.IdPropertyTrace);

            modelBuilder.Entity<Property>()
                    .HasOne(p => p.Owner)
                    .WithMany(o => o.Properties)
                    .HasForeignKey(p => p.IdOwner);

            //exact table name
            modelBuilder.Entity<Owner>().ToTable("Owner"); 
            modelBuilder.Entity<Property>().ToTable("Property");
            modelBuilder.Entity<PropertyImage>().ToTable("PropertyImage");
            modelBuilder.Entity<PropertyTrace>().ToTable("PropertyTrace");

            modelBuilder.Entity<PropertyFilter>().HasNoKey();

            modelBuilder.Entity<PropertyTrace>()
                .HasOne(pt => pt.Property)          
                .WithMany(p => p.PropertyTrace) 
                .HasForeignKey(pt => pt.IdProperty)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
