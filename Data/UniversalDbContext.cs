using System;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Universal_server.Models;
using Universal_server.Models.AliModels;

namespace Universal_server.Data
{
    public class UniversalDbContext : IdentityDbContext<IdentityUserData>
    {
        public UniversalDbContext(DbContextOptions<UniversalDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<BusinessRelation>()
               .HasOne(x => x.ProviderBusiness)
               .WithMany(x => x.ProviderBusinessRelations)
               .HasForeignKey(x => x.ProviderBusinessId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<BusinessRelation>()
                .HasOne(x => x.ConsumerBusiness)
                .WithMany(x => x.ConsumerBusinessRelations)
                .HasForeignKey(x => x.ConsumerBusinessId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<BusinessRelation>()
                .HasIndex(x => new
                {
                    x.ProviderBusinessId,
                    x.ConsumerBusinessId
                })
                .IsUnique();

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

            builder.Entity<Business_Activitiy>().HasKey(x => new
            {
                x.Activity_id,
                x.Business_id
            });


            builder.Entity<BusinessItem>()
                .HasOne(bi => bi.Business)
                .WithMany(b => b.BusinessItems)
                .HasForeignKey(bi => bi.BusinessId);

            builder.Entity<BusinessItem>()
                .HasOne(bi => bi.Item)
                .WithMany(i => i.BusinessItems)
                .HasForeignKey(bi => bi.ItemId);



            builder.Entity<UsersBusinesses>().HasKey(ub => new
            {
                ub.UserId,
                ub.Business_id
            });

            builder.Entity<UsersBusinesses>()
                .HasOne(u => u.UserData)
                .WithMany(ud => ud.UsersBusinesses)
                .HasForeignKey(ub => ub.UserId);

            builder.Entity<UsersBusinesses>()
                .HasOne(u => u.Business)
                .WithMany(ud => ud.UsersBusinesses)
                .HasForeignKey(ub => ub.Business_id);


            builder.Entity<Business_Service>()
                .HasOne(u => u.Business)
                .WithMany(ud => ud.Business_Services)
                .HasForeignKey(ub => ub.Business_id);

            builder.Entity<Business_Service>()
                .HasOne(u => u.Service)
                .WithMany(ud => ud.Business_Services)
                .HasForeignKey(ub => ub.Service_id);

            builder.Entity<Business_Service>()
                .HasIndex(x => new
                {
                    x.Business_id,
                    x.Service_id
                })
                .IsUnique();

            builder.Entity<Activity_Service>().HasKey(ub => new
            {
                ub.Activity_id,
                ub.Service_id
            });

            builder.Entity<Activity_Service>()
                .HasOne(u => u.Activiity)
                .WithMany(ud => ud.Activity_Services)
                .HasForeignKey(ub => ub.Activity_id);

            builder.Entity<Activity_Service>()
                .HasOne(u => u.Service)
                .WithMany(ud => ud.Activity_Services)
                .HasForeignKey(ub => ub.Service_id);

            builder.Entity<GlobalSystem>().HasIndex(sys => sys.GlobalSystemName);
            builder.Entity<Business>().HasIndex(sys => sys.Business_name);
            builder.Entity<CarModel>().HasIndex(sys => sys.CarModelName);
            builder.Entity<CarBrand>().HasIndex(sys => sys.CarBrandName);
            builder.Entity<CarYear>().HasIndex(sys => sys.CarYearNumber);
            builder.Entity<Activiity>().HasIndex(sys => sys.Description);
            builder.Entity<Service>().HasIndex(sys => sys.Description);
            builder.Entity<GlobalCustomer>().HasIndex(sys => sys.CustomerMobile);
            builder.Entity<Business_type>().HasIndex(sys => sys.Description);
            builder.Entity<Address>().HasIndex(sys => sys.Line_1);

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
        public DbSet<UsersBusinesses> UsersBusinesseses { get; set; }
        public DbSet<Business_Service> Business_Services { get; set; }
        public DbSet<Activity_Service> Activity_Services { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<ItemImage> ItemImages { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<BusinessItem> BusinessItems { get; set; }
        public DbSet<GlobalCustomer> GlobalCustomers { get; set; }
        public DbSet<Finantial_item> Finantial_items { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<UserImage> UserImages { get; set; }
        public DbSet<ItemCondition> ItemConditions { get; set; }
        public DbSet<Case> Cases { get; set; }
        public DbSet<caseImage> caseImages { get; set; }
        public DbSet<CarInfoTbl> CarInfoTbls { get; set; }
        public DbSet<Case_Service> Case_Services { get; set; }
        public DbSet<Case_Service_Notes> Case_Service_Notes { get; set; }
        public DbSet<Buseness_Customer> Buseness_Customers { get; set; }
        public DbSet<GlobalSystem> GlobalSystems { get; set; }
        public DbSet<System_Business_customer> System_Business_customers { get; set; }
        public DbSet<BusinessRelation> BusinessRelations { get; set; }
        public DbSet<Business_Owner> Business_Owners { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<GlobalMerchant> GlobalMerchants { get; set; }
        public DbSet<CarBrand> CarBrands { get; set; }
        public DbSet<CarModel> CarModels { get; set; }
        public DbSet<CarYear> CarYears { get; set; }
        public DbSet<Business_Activitiy> Business_Activitiies { get; set; }
        public DbSet<UserFolder> UserFolders { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<Business_Asset> Business_Assets { get; set; }
        public DbSet<GlobalOrder> GlobalOrders { get; set; }
        public DbSet<OredesServices> OredesServices { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Logistic> Logistics { get; set; }
        public DbSet<ShippingType> ShippingTypes { get; set; }
        public DbSet<ShippingCost> ShippingCosts { get; set; }
        public DbSet<Weight_Unit> Weight_Units { get; set; }
        public DbSet<Business_Service_Platform> Business_Service_Platforms { get; set; }
        public DbSet<UniversalOrder> UniversalOrders { get; set; }
        public DbSet<UniversalOrderService> UniversalOrderServices { get; set; }
        public DbSet<UniversalOrderStatus> UniversalOrderStatus { get; set; }
        public DbSet<OrderItems> OrderItems { get; set; }
        public DbSet<GlobalOrderDetail> GlobalOrderDetails { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<Complaint_Value> Complaint_Values { get; set; }
        public DbSet<ComplaintType> ComplaintTypes { get; set; }
        public DbSet<Cause> Causes { get; set; }
        public DbSet<Assign_type> Assign_types { get; set; }
        public DbSet<BusinessPlatforms> BusinessPlatforms { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Taaref> Taarefs { get; set; }
        public DbSet<BusinessPlatformCountries> BusinessPlatformCountriess { get; set; }
        public DbSet<ExchangrRate> ExchangrRates { get; set; }
        public DbSet<BusinessExchangeRate> BusinessExchangeRates { get; set; }
        public DbSet<BusinessGroup> BusinessGroups { get; set; }
        public DbSet<BusinessGroupBusinesses> BusinessGroupBusinesses { get; set; }
        public DbSet<Multimedia> Multimedias { get; set; }
        public DbSet<ShippingList> ShippingLists { get; set; }
        public DbSet<PlatformAccount> PlatformAccounts { get; set; }
        public DbSet<TariffDetails> TariffDetails { get; set; }
        public DbSet<PricingCart> PricingCarts { get; set; }
        public DbSet<PricingCartItem> PricingCartItems { get; set; }
    }
}
