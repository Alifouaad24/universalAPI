using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addShortCodeToItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UniversalOrderId",
                table: "OrderItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortCode",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_UniversalOrderId",
                table: "OrderItems",
                column: "UniversalOrderId");

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
                name: "FK_OrderItems_UniversalOrders_UniversalOrderId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_UniversalOrderId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "UniversalOrderId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ShortCode",
                table: "Items");
        }
    }
}
