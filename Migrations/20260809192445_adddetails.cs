using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class adddetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Case_Service_Notes_GlobalOrderDetail_GlobalOrderDetailId",
                table: "Case_Service_Notes");

            migrationBuilder.DropForeignKey(
                name: "FK_GlobalOrderDetail_GlobalOrders_GlobalOrderId",
                table: "GlobalOrderDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_GlobalOrderDetail_Items_ItemId",
                table: "GlobalOrderDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_GlobalOrderDetail_Services_Service_id",
                table: "GlobalOrderDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GlobalOrderDetail",
                table: "GlobalOrderDetail");

            migrationBuilder.RenameTable(
                name: "GlobalOrderDetail",
                newName: "GlobalOrderDetails");

            migrationBuilder.RenameIndex(
                name: "IX_GlobalOrderDetail_Service_id",
                table: "GlobalOrderDetails",
                newName: "IX_GlobalOrderDetails_Service_id");

            migrationBuilder.RenameIndex(
                name: "IX_GlobalOrderDetail_ItemId",
                table: "GlobalOrderDetails",
                newName: "IX_GlobalOrderDetails_ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_GlobalOrderDetail_GlobalOrderId",
                table: "GlobalOrderDetails",
                newName: "IX_GlobalOrderDetails_GlobalOrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GlobalOrderDetails",
                table: "GlobalOrderDetails",
                column: "GlobalOrderDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_Case_Service_Notes_GlobalOrderDetails_GlobalOrderDetailId",
                table: "Case_Service_Notes",
                column: "GlobalOrderDetailId",
                principalTable: "GlobalOrderDetails",
                principalColumn: "GlobalOrderDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalOrderDetails_GlobalOrders_GlobalOrderId",
                table: "GlobalOrderDetails",
                column: "GlobalOrderId",
                principalTable: "GlobalOrders",
                principalColumn: "GlobalOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalOrderDetails_Items_ItemId",
                table: "GlobalOrderDetails",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalOrderDetails_Services_Service_id",
                table: "GlobalOrderDetails",
                column: "Service_id",
                principalTable: "Services",
                principalColumn: "Service_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Case_Service_Notes_GlobalOrderDetails_GlobalOrderDetailId",
                table: "Case_Service_Notes");

            migrationBuilder.DropForeignKey(
                name: "FK_GlobalOrderDetails_GlobalOrders_GlobalOrderId",
                table: "GlobalOrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_GlobalOrderDetails_Items_ItemId",
                table: "GlobalOrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_GlobalOrderDetails_Services_Service_id",
                table: "GlobalOrderDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GlobalOrderDetails",
                table: "GlobalOrderDetails");

            migrationBuilder.RenameTable(
                name: "GlobalOrderDetails",
                newName: "GlobalOrderDetail");

            migrationBuilder.RenameIndex(
                name: "IX_GlobalOrderDetails_Service_id",
                table: "GlobalOrderDetail",
                newName: "IX_GlobalOrderDetail_Service_id");

            migrationBuilder.RenameIndex(
                name: "IX_GlobalOrderDetails_ItemId",
                table: "GlobalOrderDetail",
                newName: "IX_GlobalOrderDetail_ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_GlobalOrderDetails_GlobalOrderId",
                table: "GlobalOrderDetail",
                newName: "IX_GlobalOrderDetail_GlobalOrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GlobalOrderDetail",
                table: "GlobalOrderDetail",
                column: "GlobalOrderDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_Case_Service_Notes_GlobalOrderDetail_GlobalOrderDetailId",
                table: "Case_Service_Notes",
                column: "GlobalOrderDetailId",
                principalTable: "GlobalOrderDetail",
                principalColumn: "GlobalOrderDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalOrderDetail_GlobalOrders_GlobalOrderId",
                table: "GlobalOrderDetail",
                column: "GlobalOrderId",
                principalTable: "GlobalOrders",
                principalColumn: "GlobalOrderId");

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
    }
}
