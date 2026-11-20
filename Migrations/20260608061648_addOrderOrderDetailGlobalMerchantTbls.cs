using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addOrderOrderDetailGlobalMerchantTbls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GlobalMerchants",
                columns: table => new
                {
                    GlobalMerchantId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MerchantName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MerchantMob = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SmsAlert = table.Column<int>(type: "int", nullable: true),
                    UsDelivery = table.Column<int>(type: "int", nullable: true),
                    IsPublic = table.Column<int>(type: "int", nullable: true),
                    CityId = table.Column<int>(type: "int", nullable: true),
                    Bgw = table.Column<int>(type: "int", nullable: true),
                    Cities = table.Column<int>(type: "int", nullable: true),
                    Hidden = table.Column<int>(type: "int", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    Outskirts = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Branch = table.Column<int>(type: "int", nullable: true),
                    Sorting = table.Column<int>(type: "int", nullable: true),
                    Credit = table.Column<int>(type: "int", nullable: true),
                    Bypass = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalMerchants", x => x.GlobalMerchantId);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderDt = table.Column<DateOnly>(type: "date", nullable: true),
                    OrderOwner = table.Column<int>(type: "int", nullable: true),
                    WithDelivery = table.Column<int>(type: "int", nullable: true),
                    DeliveryCharges = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderStatus = table.Column<int>(type: "int", nullable: true),
                    ReceiptNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiptDt = table.Column<DateOnly>(type: "date", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    BatchId = table.Column<int>(type: "int", nullable: true),
                    ServiceCharges = table.Column<int>(type: "int", nullable: true),
                    Netrevenue = table.Column<int>(type: "int", nullable: true),
                    DeliveryCollectedBy = table.Column<int>(type: "int", nullable: true),
                    AssymblyCharges = table.Column<int>(type: "int", nullable: true),
                    AssymblyCollectedBy = table.Column<int>(type: "int", nullable: true),
                    DeliveryCleared = table.Column<int>(type: "int", nullable: true),
                    AssymblyCleared = table.Column<int>(type: "int", nullable: true),
                    DeliveryNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderCaseId = table.Column<int>(type: "int", nullable: true),
                    Hxcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SmsnotificationDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TaskStatusId = table.Column<int>(type: "int", nullable: true),
                    FinanceStatus = table.Column<int>(type: "int", nullable: true),
                    Closed = table.Column<int>(type: "int", nullable: true),
                    Totalqty = table.Column<int>(type: "int", nullable: true),
                    NetAmount = table.Column<int>(type: "int", nullable: true),
                    BookingDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveryDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OnlineOrder = table.Column<int>(type: "int", nullable: true),
                    FullyPackage = table.Column<int>(type: "int", nullable: true),
                    MerchantId = table.Column<int>(type: "int", nullable: true),
                    BookingSource = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BonusIq = table.Column<int>(type: "int", nullable: true),
                    LastBeforeStatusId = table.Column<int>(type: "int", nullable: true),
                    AdvancePayment = table.Column<int>(type: "int", nullable: true),
                    AgentUserId = table.Column<int>(type: "int", nullable: true),
                    Collected = table.Column<int>(type: "int", nullable: true),
                    Transferred = table.Column<int>(type: "int", nullable: true),
                    Source = table.Column<int>(type: "int", nullable: true),
                    FinancAdj = table.Column<int>(type: "int", nullable: true),
                    CollectedByOwner = table.Column<int>(type: "int", nullable: true),
                    AdminBonus = table.Column<int>(type: "int", nullable: true),
                    WithAgent = table.Column<int>(type: "int", nullable: true),
                    AdminAdj = table.Column<int>(type: "int", nullable: true),
                    SortFeesAdj = table.Column<int>(type: "int", nullable: true),
                    CompSortAdj = table.Column<int>(type: "int", nullable: true),
                    LocalDeliveryAdj = table.Column<int>(type: "int", nullable: true),
                    CompDeliveryAdj = table.Column<int>(type: "int", nullable: true),
                    ActualDeliveryDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Aliexbatchid = table.Column<int>(type: "int", nullable: true),
                    Returned = table.Column<int>(type: "int", nullable: true),
                    WithAgentId = table.Column<int>(type: "int", nullable: true),
                    PickupDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TaskWithAgentId = table.Column<int>(type: "int", nullable: true),
                    CollectionDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CloseAcknoledgeDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LinkedAdvNorderNo = table.Column<int>(type: "int", nullable: true),
                    BonusDeduct = table.Column<int>(type: "int", nullable: true),
                    DeductProgressed = table.Column<int>(type: "int", nullable: true),
                    BranchId = table.Column<int>(type: "int", nullable: true),
                    ToWarehouse = table.Column<int>(type: "int", nullable: true),
                    CityReceipt = table.Column<int>(type: "int", nullable: true),
                    PageReturnArrange = table.Column<int>(type: "int", nullable: true),
                    Acknoledged = table.Column<int>(type: "int", nullable: true),
                    PriceLog = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdvOrder = table.Column<int>(type: "int", nullable: true),
                    CollectionAdjustmentDt = table.Column<DateOnly>(type: "date", nullable: true),
                    IsCredit = table.Column<int>(type: "int", nullable: true),
                    ReturnDt = table.Column<DateOnly>(type: "date", nullable: true),
                    PreClose = table.Column<int>(type: "int", nullable: true),
                    CenrtralBankPrice = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Orders_GlobalCustomers_OrderOwner",
                        column: x => x.OrderOwner,
                        principalTable: "GlobalCustomers",
                        principalColumn: "GlobalCustomerId");
                });

            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    OrderDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNo = table.Column<int>(type: "int", nullable: true),
                    MerchantId = table.Column<int>(type: "int", nullable: true),
                    ItemCode = table.Column<int>(type: "int", nullable: true),
                    Value = table.Column<int>(type: "int", nullable: true),
                    Qty = table.Column<int>(type: "int", nullable: true),
                    OriginalAmount = table.Column<int>(type: "int", nullable: true),
                    Size = table.Column<int>(type: "int", nullable: true),
                    OnlineOrder = table.Column<int>(type: "int", nullable: true),
                    RestPaid = table.Column<int>(type: "int", nullable: true),
                    ValueCollecedBy = table.Column<int>(type: "int", nullable: true),
                    RestPaidCollectedBy = table.Column<int>(type: "int", nullable: true),
                    ValueCleared = table.Column<int>(type: "int", nullable: true),
                    RestPaidCleared = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CollectionFeesIq = table.Column<int>(type: "int", nullable: true),
                    DeliveryFeesIq = table.Column<int>(type: "int", nullable: true),
                    TaskStatusId = table.Column<int>(type: "int", nullable: true),
                    FinanceStatus = table.Column<int>(type: "int", nullable: true),
                    TrackingNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InsertDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BookingDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Missing = table.Column<int>(type: "int", nullable: true),
                    Returned = table.Column<int>(type: "int", nullable: true),
                    Sorted = table.Column<int>(type: "int", nullable: true),
                    Removed = table.Column<int>(type: "int", nullable: true),
                    Whs = table.Column<int>(type: "int", nullable: true),
                    BonusUs = table.Column<int>(type: "int", nullable: true),
                    SourcePrice = table.Column<int>(type: "int", nullable: true),
                    WebsitePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Adjusted = table.Column<int>(type: "int", nullable: true),
                    PageReturn = table.Column<int>(type: "int", nullable: true),
                    Preorder = table.Column<int>(type: "int", nullable: true),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TempMissing = table.Column<int>(type: "int", nullable: true),
                    WhsBatchId = table.Column<int>(type: "int", nullable: true),
                    Whsid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.OrderDetailId);
                    table.ForeignKey(
                        name: "FK_OrderDetails_GlobalMerchants_MerchantId",
                        column: x => x.MerchantId,
                        principalTable: "GlobalMerchants",
                        principalColumn: "GlobalMerchantId");
                    table.ForeignKey(
                        name: "FK_OrderDetails_Items_ItemCode",
                        column: x => x.ItemCode,
                        principalTable: "Items",
                        principalColumn: "ItemId");
                    table.ForeignKey(
                        name: "FK_OrderDetails_Orders_OrderNo",
                        column: x => x.OrderNo,
                        principalTable: "Orders",
                        principalColumn: "OrderId");
                    table.ForeignKey(
                        name: "FK_OrderDetails_Sizes_Size",
                        column: x => x.Size,
                        principalTable: "Sizes",
                        principalColumn: "Size_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ItemCode",
                table: "OrderDetails",
                column: "ItemCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_MerchantId",
                table: "OrderDetails",
                column: "MerchantId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderNo",
                table: "OrderDetails",
                column: "OrderNo");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_Size",
                table: "OrderDetails",
                column: "Size");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderOwner",
                table: "Orders",
                column: "OrderOwner");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "GlobalMerchants");

            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
