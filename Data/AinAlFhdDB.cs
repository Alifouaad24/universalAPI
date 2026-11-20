using System;
using System.Collections.Generic;
using AinAlfahd.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Universal_server.Models;
using Universal_server.Models.AinAlFHDModels;
using Area = AinAlfahd.Models.Area;
using City = AinAlfahd.Models.City;

namespace AinAlfahd.Data;

public partial class MasterDBContext : IdentityDbContext<IdentityUser>
{

    public MasterDBContext(DbContextOptions<MasterDBContext> options)
        : base(options)
    {
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
    }

    public DbSet<CustomerService> CustomerServices { get; set; }
    public DbSet<Area> Areas { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public virtual DbSet<ShippingTypes> ShippingTypes { get; set; }
    public virtual DbSet<AinAlfhdItem> AinAlfhdItem { get; set; }
    public virtual DbSet<AinAlfhdItemImages> AinAlfhdItemImages { get; set; }
    public virtual DbSet<AinAlFhdOrderDetail> AinAlFhdOrderDetails { get; set; }
    public virtual DbSet<AinAlFhdOrder> AinAlFhdOrders { get; set; }
    public virtual DbSet<AinAlFhdSize> AinAlFhdSizes { get; set; }
    public virtual DbSet<Merchant> Merchants { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);

        // /////////////////////////////////////////////////////////////////////////////////

        modelBuilder.Entity<CustomerService>()
            .HasKey(cs => new { cs.CustomerId, cs.ServiceId });

        modelBuilder.Entity<CustomerService>()
            .HasOne(cs => cs.Customer)
            .WithMany(c => c.CustomerServices)
            .HasForeignKey(cs => cs.CustomerId);

        modelBuilder.Entity<CustomerService>()
            .HasOne(cs => cs.Service)
            .WithMany(s => s.CustomerServices)
            .HasForeignKey(cs => cs.ServiceId);

        modelBuilder.Entity<Customer>()
        .HasIndex(c => c.CustMob)
        .IsUnique();


        /////////////////////////////////////////////////////////////

        modelBuilder.UseCollation("Arabic_CI_AS");
        modelBuilder.Entity<AinAlfhdItem>(entity =>
        {
            entity.ToTable("items");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ArDesc)
                .HasMaxLength(150)
                .HasColumnName("AR_DESC");
            entity.Property(e => e.Assymbly).HasColumnName("assymbly");
            entity.Property(e => e.Comb).HasColumnName("comb");
            entity.Property(e => e.Cost).HasColumnName("cost");
            entity.Property(e => e.EngName)
                .HasMaxLength(50)
                .HasColumnName("eng_name");
            entity.Property(e => e.Fr)
                .HasMaxLength(50)
                .HasColumnName("FR");
            entity.Property(e => e.H).HasColumnName("h");
            entity.Property(e => e.ImgUrl)
                .HasMaxLength(250)
                .HasColumnName("img_url");
            entity.Property(e => e.L).HasColumnName("l");
            entity.Property(e => e.PCode)
                .HasMaxLength(50)
                .HasColumnName("p_code");
            entity.Property(e => e.SitePrice)
                .HasColumnType("money")
                .HasColumnName("site_price");
            entity.Property(e => e.SubId).HasColumnName("sub_id");
            entity.Property(e => e.W).HasColumnName("w");
            entity.Property(e => e.WebUrl)
                .HasMaxLength(250)
                .HasColumnName("web_url");
            entity.Property(e => e.Weight).HasColumnName("weight");
        });

        modelBuilder.Entity<Area>(entity =>
        {
            entity.ToTable("areas");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CityId).HasColumnName("city_id");
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .HasColumnName("description");
            entity.Property(e => e.Sector).HasColumnName("sector");
            entity.Property(e => e.Spec).HasColumnName("spec");
            entity.Property(e => e.Zone).HasColumnName("zone");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.ToTable("cities");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .HasColumnName("description");
            entity.Property(e => e.IsNorth).HasColumnName("isNorth");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("customers");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CustArea).HasColumnName("cust_area");
            entity.Property(e => e.CustCity).HasColumnName("cust_city");
            entity.Property(e => e.CustLandmark)
                .HasMaxLength(150)
                .HasColumnName("cust_landmark");
            entity.Property(e => e.CustMob)
                .HasMaxLength(15)
                .HasColumnName("cust_mob");
            entity.Property(e => e.CustMob2)
                .HasMaxLength(15)
                .HasColumnName("cust_mob2");
            entity.Property(e => e.CustName)
                .HasMaxLength(150)
                .HasColumnName("cust_name");
            entity.Property(e => e.CustProfile)
                .HasMaxLength(300)
                .HasColumnName("cust_profile");
            entity.Property(e => e.CustStatus).HasColumnName("cust_status");
            entity.Property(e => e.Fbid)
                .HasMaxLength(50)
                .HasColumnName("fbid");
            entity.Property(e => e.FullPackage).HasColumnName("full_package");
            entity.Property(e => e.Gisurl)
                .HasMaxLength(50)
                .HasColumnName("gisurl");
            entity.Property(e => e.Hexcode)
                .HasMaxLength(50)
                .HasColumnName("hexcode");
            entity.Property(e => e.InsertDt).HasColumnName("insert_dt");
            entity.Property(e => e.Lat)
                .HasMaxLength(150)
                .HasColumnName("lat");
            entity.Property(e => e.Lon).HasColumnName("lon");
        });


        modelBuilder.Entity<AinAlFhdOrder>().Property(o => o.OrderOwner).HasColumnName("order_owner");
        modelBuilder.Entity<AinAlFhdOrder>().Property(o => o.MerchantId).HasColumnName("merchant_id");
        modelBuilder.Entity<AinAlFhdOrder>().Property(o => o.ActualDeliveryDt).HasColumnName("actual_delivery_dt");
        modelBuilder.Entity<AinAlFhdOrderDetail>().Property(o => o.OrderNo).HasColumnName("order_no");

        modelBuilder.Entity<AinAlFhdOrder>(entity =>
        {
            entity.ToTable("orders");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Acknoledged).HasColumnName("acknoledged");
            entity.Property(e => e.ActualDeliveryDt)
                .HasColumnType("smalldatetime")
                .HasColumnName("actual_delivery_dt");
            entity.Property(e => e.AdminAdj).HasColumnName("admin_adj");
            entity.Property(e => e.AdminBonus).HasColumnName("admin_bonus");
            entity.Property(e => e.AdvOrder).HasColumnName("adv_order");
            entity.Property(e => e.AdvancePayment).HasColumnName("advance_payment");
            entity.Property(e => e.AgentUserId).HasColumnName("agent_user_id");
            entity.Property(e => e.Aliexbatchid).HasColumnName("aliexbatchid");
            entity.Property(e => e.AssymblyCharges).HasColumnName("assymbly_charges");
            entity.Property(e => e.AssymblyCleared).HasColumnName("assymblyCleared");
            entity.Property(e => e.AssymblyCollectedBy).HasColumnName("assymbly_collected_by");
            entity.Property(e => e.BatchId).HasColumnName("BatchID");
            entity.Property(e => e.BonusDeduct).HasColumnName("bonus_deduct");
            entity.Property(e => e.BonusIq).HasColumnName("bonusIQ");
            entity.Property(e => e.BookingDt)
                .HasColumnType("smalldatetime")
                .HasColumnName("booking_dt");
            entity.Property(e => e.BookingSource)
                .HasColumnType("money")
                .HasColumnName("booking_source");
            entity.Property(e => e.BranchId).HasColumnName("branch_id");
            entity.Property(e => e.CenrtralBankPrice).HasColumnName("cenrtral_bank_price_135");
            entity.Property(e => e.CityReceipt).HasColumnName("city_receipt");
            entity.Property(e => e.CloseAcknoledgeDt)
                .HasColumnType("smalldatetime")
                .HasColumnName("close_acknoledge_dt");
            entity.Property(e => e.Closed).HasColumnName("closed");
            entity.Property(e => e.Collected).HasColumnName("collected");
            entity.Property(e => e.CollectedByOwner).HasColumnName("collected_by_owner");
            entity.Property(e => e.CollectionAdjustmentDt).HasColumnName("collection_adjustment_dt");
            entity.Property(e => e.CollectionDt)
                .HasColumnType("smalldatetime")
                .HasColumnName("collection_dt");
            entity.Property(e => e.CompDeliveryAdj).HasColumnName("comp_delivery_adj");
            entity.Property(e => e.CompSortAdj).HasColumnName("comp_sort_adj");
            entity.Property(e => e.DeductProgressed).HasColumnName("deduct_progressed");
            entity.Property(e => e.DeliveryCharges).HasColumnName("delivery_charges");
            entity.Property(e => e.DeliveryCleared).HasColumnName("deliveryCleared");
            entity.Property(e => e.DeliveryCollectedBy).HasColumnName("deliveryCollectedBy");
            entity.Property(e => e.DeliveryDt)
                .HasColumnType("smalldatetime")
                .HasColumnName("delivery_dt");
            entity.Property(e => e.DeliveryNotes)
                .HasMaxLength(250)
                .HasColumnName("delivery_notes");
            entity.Property(e => e.FinancAdj).HasColumnName("financ_adj");
            entity.Property(e => e.FinanceStatus).HasColumnName("finance_status");
            entity.Property(e => e.FullyPackage).HasColumnName("fully_package");
            entity.Property(e => e.Hxcode)
                .HasMaxLength(50)
                .HasColumnName("hxcode");
            entity.Property(e => e.IsCredit).HasColumnName("is_credit");
            entity.Property(e => e.LastBeforeStatusId).HasColumnName("last_before_status_id");
            entity.Property(e => e.LinkedAdvNorderNo).HasColumnName("linked_adv_norder_no");
            entity.Property(e => e.LocalDeliveryAdj).HasColumnName("local_delivery_adj");
            entity.Property(e => e.MerchantId).HasColumnName("merchant_id");
            entity.Property(e => e.Netrevenue).HasColumnName("netrevenue");
            entity.Property(e => e.Notes)
                .HasMaxLength(650)
                .HasColumnName("notes");
            entity.Property(e => e.OnlineOrder).HasColumnName("online_order");
            entity.Property(e => e.OrderCaseId).HasColumnName("order_case_id");
            entity.Property(e => e.OrderDt).HasColumnName("order_dt");
            entity.Property(e => e.OrderOwner).HasColumnName("order_owner");
            entity.Property(e => e.OrderStatus).HasColumnName("order_status");
            entity.Property(e => e.PageReturnArrange).HasColumnName("page_return_arrange");
            entity.Property(e => e.PickupDt)
                .HasColumnType("smalldatetime")
                .HasColumnName("pickup_dt");
            entity.Property(e => e.PreClose).HasColumnName("pre_close");
            entity.Property(e => e.PriceLog)
                .HasMaxLength(300)
                .HasColumnName("price_log");
            entity.Property(e => e.ReceiptDt).HasColumnName("receipt_dt");
            entity.Property(e => e.ReceiptNo)
                .HasMaxLength(50)
                .HasColumnName("receipt_no");
            entity.Property(e => e.ReturnDt).HasColumnName("return_dt");
            entity.Property(e => e.Returned).HasColumnName("returned");
            entity.Property(e => e.ServiceCharges).HasColumnName("service_charges");
            entity.Property(e => e.SmsnotificationDt)
                .HasColumnType("smalldatetime")
                .HasColumnName("smsnotification_dt");
            entity.Property(e => e.SortFeesAdj).HasColumnName("sort_fees_adj");
            entity.Property(e => e.Source).HasColumnName("source");
            entity.Property(e => e.TaskStatusId).HasColumnName("task_status_id");
            entity.Property(e => e.TaskWithAgentId).HasColumnName("task_with_agent_id");
            entity.Property(e => e.ToWarehouse).HasColumnName("to_warehouse");
            entity.Property(e => e.Totalqty).HasColumnName("totalqty");
            entity.Property(e => e.Transferred).HasColumnName("transferred");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.WithAgent).HasColumnName("with_agent");
            entity.Property(e => e.WithAgentId).HasColumnName("with_agent_id");
            entity.Property(e => e.WithDelivery).HasColumnName("with_delivery");
        });

        modelBuilder.Entity<AinAlFhdOrderDetail>(entity =>
        {
            entity.ToTable("order_details");

            entity.Property(e => e.OrderDetailId).HasColumnName("id");
            entity.Property(e => e.Adjusted).HasColumnName("adjusted");
            entity.Property(e => e.BonusUs).HasColumnName("bonusUS");
            entity.Property(e => e.BookingDt)
                .HasColumnType("smalldatetime")
                .HasColumnName("booking_dt");
            entity.Property(e => e.CollectionFeesIq).HasColumnName("collection_feesIQ");
            entity.Property(e => e.DeliveryFeesIq).HasColumnName("Delivery_feesIQ");
            entity.Property(e => e.FinanceStatus).HasColumnName("finance_status");
            entity.Property(e => e.InsertDt)
                .HasColumnType("smalldatetime")
                .HasColumnName("insert_dt");
            entity.Property(e => e.ItemCode).HasColumnName("item_code");
            entity.Property(e => e.Missing).HasColumnName("missing");
            entity.Property(e => e.Notes)
                .HasMaxLength(50)
                .HasColumnName("notes");
            entity.Property(e => e.OnlineOrder).HasColumnName("online_order");
            entity.Property(e => e.OrderId)
                .HasMaxLength(50)
                .HasColumnName("OrderID");
            entity.Property(e => e.OrderNo).HasColumnName("order_no");
            entity.Property(e => e.OriginalAmount).HasColumnName("original_amount");
            entity.Property(e => e.PageReturn).HasColumnName("page_return");
            entity.Property(e => e.Preorder).HasColumnName("preorder");
            entity.Property(e => e.Qty).HasColumnName("qty");
            entity.Property(e => e.Removed).HasColumnName("removed");
            entity.Property(e => e.Returned).HasColumnName("returned");
            entity.Property(e => e.Size_id).HasColumnName("size");
            entity.Property(e => e.Sorted).HasColumnName("sorted");
            entity.Property(e => e.SourcePrice).HasColumnName("source_price");
            entity.Property(e => e.TaskStatusId).HasColumnName("task_status_id");
            entity.Property(e => e.TempMissing).HasColumnName("temp_missing");
            entity.Property(e => e.TrackingNo)
                .HasMaxLength(50)
                .HasColumnName("tracking_no");
            entity.Property(e => e.Value).HasColumnName("value");
            entity.Property(e => e.ValueCleared).HasColumnName("valueCleared");
            entity.Property(e => e.ValueCollecedBy).HasColumnName("valueCollecedBy");
            entity.Property(e => e.WebsitePrice)
                .HasColumnType("money")
                .HasColumnName("website_price");
            entity.Property(e => e.Whs).HasColumnName("whs");
            entity.Property(e => e.WhsBatchId).HasColumnName("whs_batch_id");
            entity.Property(e => e.Whsid).HasColumnName("whsid");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
