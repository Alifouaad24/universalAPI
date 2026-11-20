using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addOrderDetailyhss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GlobalOrders_GlobalOrderDetail_GlobalOrderDetailId",
                table: "GlobalOrders");

            migrationBuilder.DropIndex(
                name: "IX_GlobalOrders_GlobalOrderDetailId",
                table: "GlobalOrders");

            migrationBuilder.DropColumn(
                name: "GlobalOrderDetailId",
                table: "GlobalOrders");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalOrderDetail_GlobalOrderId",
                table: "GlobalOrderDetail",
                column: "GlobalOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalOrderDetail_GlobalOrders_GlobalOrderId",
                table: "GlobalOrderDetail",
                column: "GlobalOrderId",
                principalTable: "GlobalOrders",
                principalColumn: "GlobalOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GlobalOrderDetail_GlobalOrders_GlobalOrderId",
                table: "GlobalOrderDetail");

            migrationBuilder.DropIndex(
                name: "IX_GlobalOrderDetail_GlobalOrderId",
                table: "GlobalOrderDetail");

            migrationBuilder.AddColumn<int>(
                name: "GlobalOrderDetailId",
                table: "GlobalOrders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GlobalOrders_GlobalOrderDetailId",
                table: "GlobalOrders",
                column: "GlobalOrderDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalOrders_GlobalOrderDetail_GlobalOrderDetailId",
                table: "GlobalOrders",
                column: "GlobalOrderDetailId",
                principalTable: "GlobalOrderDetail",
                principalColumn: "GlobalOrderDetailId");
        }
    }
}
