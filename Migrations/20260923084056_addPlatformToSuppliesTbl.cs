using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addPlatformToSuppliesTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                table: "UniversalOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Platform_id",
                table: "Suppliers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UniversalOrders_SupplierId",
                table: "UniversalOrders",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_Platform_id",
                table: "Suppliers",
                column: "Platform_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Suppliers_Platforms_Platform_id",
                table: "Suppliers",
                column: "Platform_id",
                principalTable: "Platforms",
                principalColumn: "Platform_id");

            migrationBuilder.AddForeignKey(
                name: "FK_UniversalOrders_Suppliers_SupplierId",
                table: "UniversalOrders",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "SupplierId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Suppliers_Platforms_Platform_id",
                table: "Suppliers");

            migrationBuilder.DropForeignKey(
                name: "FK_UniversalOrders_Suppliers_SupplierId",
                table: "UniversalOrders");

            migrationBuilder.DropIndex(
                name: "IX_UniversalOrders_SupplierId",
                table: "UniversalOrders");

            migrationBuilder.DropIndex(
                name: "IX_Suppliers_Platform_id",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "UniversalOrders");

            migrationBuilder.DropColumn(
                name: "Platform_id",
                table: "Suppliers");
        }
    }
}
