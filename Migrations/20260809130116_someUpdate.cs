using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class someUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "GlobalOrders");

            migrationBuilder.AddColumn<int>(
                name: "Business_id",
                table: "UniversalOrderStatus",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GlobalOrderId",
                table: "OrderItems",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Schedule_time",
                table: "GlobalOrders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Cart_value",
                table: "GlobalOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "GlobalOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "CreatedDate",
                table: "GlobalOrders",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: "Modified_by",
                table: "GlobalOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderStatusId",
                table: "GlobalOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Platform_id",
                table: "GlobalOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShippingTypeId",
                table: "GlobalOrders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UniversalOrderStatus_Business_id",
                table: "UniversalOrderStatus",
                column: "Business_id");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_GlobalOrderId",
                table: "OrderItems",
                column: "GlobalOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalOrders_OrderStatusId",
                table: "GlobalOrders",
                column: "OrderStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalOrders_Platform_id",
                table: "GlobalOrders",
                column: "Platform_id");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalOrders_ShippingTypeId",
                table: "GlobalOrders",
                column: "ShippingTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalOrders_Platforms_Platform_id",
                table: "GlobalOrders",
                column: "Platform_id",
                principalTable: "Platforms",
                principalColumn: "Platform_id");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalOrders_ShippingTypes_ShippingTypeId",
                table: "GlobalOrders",
                column: "ShippingTypeId",
                principalTable: "ShippingTypes",
                principalColumn: "ShippingTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalOrders_UniversalOrderStatus_OrderStatusId",
                table: "GlobalOrders",
                column: "OrderStatusId",
                principalTable: "UniversalOrderStatus",
                principalColumn: "OrderStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_GlobalOrders_GlobalOrderId",
                table: "OrderItems",
                column: "GlobalOrderId",
                principalTable: "GlobalOrders",
                principalColumn: "GlobalOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_UniversalOrderStatus_Businesses_Business_id",
                table: "UniversalOrderStatus",
                column: "Business_id",
                principalTable: "Businesses",
                principalColumn: "Business_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GlobalOrders_Platforms_Platform_id",
                table: "GlobalOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_GlobalOrders_ShippingTypes_ShippingTypeId",
                table: "GlobalOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_GlobalOrders_UniversalOrderStatus_OrderStatusId",
                table: "GlobalOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_GlobalOrders_GlobalOrderId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_UniversalOrderStatus_Businesses_Business_id",
                table: "UniversalOrderStatus");

            migrationBuilder.DropIndex(
                name: "IX_UniversalOrderStatus_Business_id",
                table: "UniversalOrderStatus");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_GlobalOrderId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_GlobalOrders_OrderStatusId",
                table: "GlobalOrders");

            migrationBuilder.DropIndex(
                name: "IX_GlobalOrders_Platform_id",
                table: "GlobalOrders");

            migrationBuilder.DropIndex(
                name: "IX_GlobalOrders_ShippingTypeId",
                table: "GlobalOrders");

            migrationBuilder.DropColumn(
                name: "Business_id",
                table: "UniversalOrderStatus");

            migrationBuilder.DropColumn(
                name: "GlobalOrderId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "Cart_value",
                table: "GlobalOrders");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "GlobalOrders");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "GlobalOrders");

            migrationBuilder.DropColumn(
                name: "Modified_by",
                table: "GlobalOrders");

            migrationBuilder.DropColumn(
                name: "OrderStatusId",
                table: "GlobalOrders");

            migrationBuilder.DropColumn(
                name: "Platform_id",
                table: "GlobalOrders");

            migrationBuilder.DropColumn(
                name: "ShippingTypeId",
                table: "GlobalOrders");

            migrationBuilder.AlterColumn<string>(
                name: "Schedule_time",
                table: "GlobalOrders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "GlobalOrders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
