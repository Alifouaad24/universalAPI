using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addExchang : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExchangrRates",
                columns: table => new
                {
                    ExchangrRateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrencySourceId = table.Column<int>(type: "int", nullable: true),
                    CurrencyTargetId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangrRates", x => x.ExchangrRateId);
                    table.ForeignKey(
                        name: "FK_ExchangrRates_Currencies_CurrencySourceId",
                        column: x => x.CurrencySourceId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId");
                    table.ForeignKey(
                        name: "FK_ExchangrRates_Currencies_CurrencyTargetId",
                        column: x => x.CurrencyTargetId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId");
                });

            migrationBuilder.CreateTable(
                name: "BusinessExchangeRates",
                columns: table => new
                {
                    BusinessExchangeRateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Business_id = table.Column<int>(type: "int", nullable: true),
                    ExchangrRateId = table.Column<int>(type: "int", nullable: true),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessExchangeRates", x => x.BusinessExchangeRateId);
                    table.ForeignKey(
                        name: "FK_BusinessExchangeRates_Businesses_Business_id",
                        column: x => x.Business_id,
                        principalTable: "Businesses",
                        principalColumn: "Business_id");
                    table.ForeignKey(
                        name: "FK_BusinessExchangeRates_ExchangrRates_ExchangrRateId",
                        column: x => x.ExchangrRateId,
                        principalTable: "ExchangrRates",
                        principalColumn: "ExchangrRateId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessExchangeRates_Business_id",
                table: "BusinessExchangeRates",
                column: "Business_id");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessExchangeRates_ExchangrRateId",
                table: "BusinessExchangeRates",
                column: "ExchangrRateId");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangrRates_CurrencySourceId_CurrencyTargetId",
                table: "ExchangrRates",
                columns: new[] { "CurrencySourceId", "CurrencyTargetId" },
                unique: true,
                filter: "[CurrencySourceId] IS NOT NULL AND [CurrencyTargetId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangrRates_CurrencyTargetId",
                table: "ExchangrRates",
                column: "CurrencyTargetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessExchangeRates");

            migrationBuilder.DropTable(
                name: "ExchangrRates");
        }
    }
}
