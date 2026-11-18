using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class setModifyByAsNullInUniversalOrderTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_UniversalOrders_UniversalOrderId",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "UniversalOrderId",
                table: "OrderItems",
                newName: "Service_id");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_UniversalOrderId",
                table: "OrderItems",
                newName: "IX_OrderItems_Service_id");

            migrationBuilder.AlterColumn<string>(
                name: "Modified_by",
                table: "UniversalOrders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "OrderItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Size_id",
                table: "OrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_Size_id",
                table: "OrderItems",
                column: "Size_id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Services_Service_id",
                table: "OrderItems",
                column: "Service_id",
                principalTable: "Services",
                principalColumn: "Service_id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Sizes_Size_id",
                table: "OrderItems",
                column: "Size_id",
                principalTable: "Sizes",
                principalColumn: "Size_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_UniversalOrders_OrderId",
                table: "OrderItems",
                column: "OrderId",
                principalTable: "UniversalOrders",
                principalColumn: "UniversalOrderId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Services_Service_id",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Sizes_Size_id",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_UniversalOrders_OrderId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_Size_id",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "Size_id",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "Service_id",
                table: "OrderItems",
                newName: "UniversalOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_Service_id",
                table: "OrderItems",
                newName: "IX_OrderItems_UniversalOrderId");

            migrationBuilder.AlterColumn<string>(
                name: "Modified_by",
                table: "UniversalOrders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_UniversalOrders_UniversalOrderId",
                table: "OrderItems",
                column: "UniversalOrderId",
                principalTable: "UniversalOrders",
                principalColumn: "UniversalOrderId");
        }
    }
}
