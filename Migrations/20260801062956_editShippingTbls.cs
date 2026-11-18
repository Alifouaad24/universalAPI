using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class editShippingTbls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CostAmount",
                table: "ShippingCosts",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LogisticId",
                table: "ShippingCosts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SaleAmount",
                table: "ShippingCosts",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingCosts_LogisticId",
                table: "ShippingCosts",
                column: "LogisticId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingCosts_Logistics_LogisticId",
                table: "ShippingCosts",
                column: "LogisticId",
                principalTable: "Logistics",
                principalColumn: "LogisticId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingCosts_Logistics_LogisticId",
                table: "ShippingCosts");

            migrationBuilder.DropIndex(
                name: "IX_ShippingCosts_LogisticId",
                table: "ShippingCosts");

            migrationBuilder.DropColumn(
                name: "CostAmount",
                table: "ShippingCosts");

            migrationBuilder.DropColumn(
                name: "LogisticId",
                table: "ShippingCosts");

            migrationBuilder.DropColumn(
                name: "SaleAmount",
                table: "ShippingCosts");
        }
    }
}
