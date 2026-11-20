using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class movePreOrdersToGlobalOrderTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_UniversalOrders_OrderId",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "OrderItems",
                newName: "GlobalOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                newName: "IX_OrderItems_GlobalOrderId");

            migrationBuilder.AddColumn<int>(
                name: "UniversalOrderId",
                table: "OrderItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                table: "GlobalOrders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_UniversalOrderId",
                table: "OrderItems",
                column: "UniversalOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalOrders_SupplierId",
                table: "GlobalOrders",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalOrders_Suppliers_SupplierId",
                table: "GlobalOrders",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_GlobalOrders_GlobalOrderId",
                table: "OrderItems",
                column: "GlobalOrderId",
                principalTable: "GlobalOrders",
                principalColumn: "GlobalOrderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_UniversalOrders_UniversalOrderId",
                table: "OrderItems",
                column: "UniversalOrderId",
                principalTable: "UniversalOrders",
                principalColumn: "UniversalOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GlobalOrders_Suppliers_SupplierId",
                table: "GlobalOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_GlobalOrders_GlobalOrderId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_UniversalOrders_UniversalOrderId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_UniversalOrderId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_GlobalOrders_SupplierId",
                table: "GlobalOrders");

            migrationBuilder.DropColumn(
                name: "UniversalOrderId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "GlobalOrders");

            migrationBuilder.RenameColumn(
                name: "GlobalOrderId",
                table: "OrderItems",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_GlobalOrderId",
                table: "OrderItems",
                newName: "IX_OrderItems_OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_UniversalOrders_OrderId",
                table: "OrderItems",
                column: "OrderId",
                principalTable: "UniversalOrders",
                principalColumn: "UniversalOrderId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
