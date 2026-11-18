using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addUniversalOrdersTblsSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UniversalOrderStatus",
                columns: table => new
                {
                    OrderStatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusEn = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniversalOrderStatus", x => x.OrderStatusId);
                });

            migrationBuilder.CreateTable(
                name: "UniversalOrders",
                columns: table => new
                {
                    UniversalOrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Platform_id = table.Column<int>(type: "int", nullable: true),
                    Business_id = table.Column<int>(type: "int", nullable: true),
                    GlobalCustomerId = table.Column<int>(type: "int", nullable: true),
                    ShippingTypeId = table.Column<int>(type: "int", nullable: true),
                    OrderStatusId = table.Column<int>(type: "int", nullable: true),
                    Cart_value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified_by = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniversalOrders", x => x.UniversalOrderId);
                    table.ForeignKey(
                        name: "FK_UniversalOrders_Businesses_Business_id",
                        column: x => x.Business_id,
                        principalTable: "Businesses",
                        principalColumn: "Business_id");
                    table.ForeignKey(
                        name: "FK_UniversalOrders_GlobalCustomers_GlobalCustomerId",
                        column: x => x.GlobalCustomerId,
                        principalTable: "GlobalCustomers",
                        principalColumn: "GlobalCustomerId");
                    table.ForeignKey(
                        name: "FK_UniversalOrders_Platforms_Platform_id",
                        column: x => x.Platform_id,
                        principalTable: "Platforms",
                        principalColumn: "Platform_id");
                    table.ForeignKey(
                        name: "FK_UniversalOrders_ShippingTypes_ShippingTypeId",
                        column: x => x.ShippingTypeId,
                        principalTable: "ShippingTypes",
                        principalColumn: "ShippingTypeId");
                    table.ForeignKey(
                        name: "FK_UniversalOrders_UniversalOrderStatus_OrderStatusId",
                        column: x => x.OrderStatusId,
                        principalTable: "UniversalOrderStatus",
                        principalColumn: "OrderStatusId");
                });

            migrationBuilder.CreateTable(
                name: "UniversalOrderServices",
                columns: table => new
                {
                    OrderServiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Service_id = table.Column<int>(type: "int", nullable: true),
                    UniversalOrderId = table.Column<int>(type: "int", nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Size_id = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniversalOrderServices", x => x.OrderServiceId);
                    table.ForeignKey(
                        name: "FK_UniversalOrderServices_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UniversalOrderServices_Services_Service_id",
                        column: x => x.Service_id,
                        principalTable: "Services",
                        principalColumn: "Service_id");
                    table.ForeignKey(
                        name: "FK_UniversalOrderServices_UniversalOrders_UniversalOrderId",
                        column: x => x.UniversalOrderId,
                        principalTable: "UniversalOrders",
                        principalColumn: "UniversalOrderId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UniversalOrders_Business_id",
                table: "UniversalOrders",
                column: "Business_id");

            migrationBuilder.CreateIndex(
                name: "IX_UniversalOrders_GlobalCustomerId",
                table: "UniversalOrders",
                column: "GlobalCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_UniversalOrders_OrderStatusId",
                table: "UniversalOrders",
                column: "OrderStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_UniversalOrders_Platform_id",
                table: "UniversalOrders",
                column: "Platform_id");

            migrationBuilder.CreateIndex(
                name: "IX_UniversalOrders_ShippingTypeId",
                table: "UniversalOrders",
                column: "ShippingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UniversalOrderServices_ItemId",
                table: "UniversalOrderServices",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_UniversalOrderServices_Service_id",
                table: "UniversalOrderServices",
                column: "Service_id");

            migrationBuilder.CreateIndex(
                name: "IX_UniversalOrderServices_UniversalOrderId",
                table: "UniversalOrderServices",
                column: "UniversalOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UniversalOrderServices");

            migrationBuilder.DropTable(
                name: "UniversalOrders");

            migrationBuilder.DropTable(
                name: "UniversalOrderStatus");
        }
    }
}
