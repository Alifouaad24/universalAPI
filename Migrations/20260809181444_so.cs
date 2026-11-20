using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class so : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_GlobalOrders_GlobalOrderId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_GlobalOrderId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "GlobalOrderId",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "GlobalOrderDetail",
                newName: "Service_id");

            migrationBuilder.AddColumn<int>(
                name: "ItemId",
                table: "GlobalOrderDetail",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GlobalOrderDetail_ItemId",
                table: "GlobalOrderDetail",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalOrderDetail_Service_id",
                table: "GlobalOrderDetail",
                column: "Service_id");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalOrderDetail_Items_ItemId",
                table: "GlobalOrderDetail",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalOrderDetail_Services_Service_id",
                table: "GlobalOrderDetail",
                column: "Service_id",
                principalTable: "Services",
                principalColumn: "Service_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GlobalOrderDetail_Items_ItemId",
                table: "GlobalOrderDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_GlobalOrderDetail_Services_Service_id",
                table: "GlobalOrderDetail");

            migrationBuilder.DropIndex(
                name: "IX_GlobalOrderDetail_ItemId",
                table: "GlobalOrderDetail");

            migrationBuilder.DropIndex(
                name: "IX_GlobalOrderDetail_Service_id",
                table: "GlobalOrderDetail");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "GlobalOrderDetail");

            migrationBuilder.RenameColumn(
                name: "Service_id",
                table: "GlobalOrderDetail",
                newName: "ServiceId");

            migrationBuilder.AddColumn<int>(
                name: "GlobalOrderId",
                table: "OrderItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_GlobalOrderId",
                table: "OrderItems",
                column: "GlobalOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_GlobalOrders_GlobalOrderId",
                table: "OrderItems",
                column: "GlobalOrderId",
                principalTable: "GlobalOrders",
                principalColumn: "GlobalOrderId");
        }
    }
}
