using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Universal_server.Models;

namespace Universal_server.Data
{
    public class UniversalDbContext : IdentityDbContext<IdentityUserData>
    {
        public UniversalDbContext(DbContextOptions<UniversalDbContext> options) : base(options)
        {}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Business_BusinessType>()
                .HasKey(x => new { x.Business_id, x.Business_type_id });

            builder.Entity<Business_BusinessType>()
                .HasOne(x => x.Business)
                .WithMany(x => x.BusinessTypes)
                .HasForeignKey(x => x.Business_id);

            builder.Entity<Business_BusinessType>()
                .HasOne(x => x.BusinessType)
                .WithMany(x => x.BusinessTypes)
                .HasForeignKey(x => x.Business_type_id);

            builder.Entity<Business_Address>()
                .HasKey(x => new { x.Business_id, x.Address_id });

            builder.Entity<Business_Address>()
                .HasOne(x => x.Business)
                .WithMany(x => x.BusinessAddresses)
                .HasForeignKey(x => x.Business_id);

            builder.Entity<Business_Address>()
                .HasOne(x => x.Address)
                .WithMany(x => x.BusinessAddresses)
                .HasForeignKey(x => x.Address_id);

            builder.Entity<Business>().Property(x => x.visible).HasDefaultValue(true);
            builder.Entity<Business_type>().Property(x => x.visible).HasDefaultValue(true);
            builder.Entity<Country>().Property(x => x.visible).HasDefaultValue(true);
            builder.Entity<Business_BusinessType>().Property(x => x.visible).HasDefaultValue(true);
            builder.Entity<Business_Address>().Property(x => x.visible).HasDefaultValue(true);
            builder.Entity<System_sectore>().Property(x => x.visible).HasDefaultValue(true);
            builder.Entity<System_gateway>().Property(x => x.visible).HasDefaultValue(true);
            builder.Entity<System_sectore_details>().Property(x => x.visible).HasDefaultValue(true);
            builder.Entity<Category>().Property(x => x.visible).HasDefaultValue(true);
            builder.Entity<Size>().Property(x => x.visible).HasDefaultValue(true);
            builder.Entity<Platform>().Property(x => x.visible).HasDefaultValue(true);
            builder.Entity<Inventory>().Property(x => x.visible).HasDefaultValue(true);
            builder.Entity<Inventory_business>().Property(x => x.visible).HasDefaultValue(true);
            builder.Entity<Inventoy_images>().Property(x => x.visible).HasDefaultValue(true);
            builder.Entity<Service>().Property(x => x.visible).HasDefaultValue(true);
        }

        public DbSet<Business> Businesses { get; set; }
        public DbSet<Business_type> Business_types { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Business_BusinessType> Business_BusinessTypes { get; set; }
        public DbSet<Business_Address> Business_Addresses { get; set; }
        public DbSet<Activiity> Activiities { get; set; }
        public DbSet<System_sectore> System_sectores { get; set; }
        public DbSet<System_gateway> System_gatewaies { get; set; }
        public DbSet<System_sectore_details> System_sectore_details { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<Platform> Platforms { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Inventory_business> Inventory_businesses { get; set; }
        public DbSet<Inventoy_images> Inventoy_images { get; set; }
        public DbSet<Service> Services { get; set; }

    }
}
