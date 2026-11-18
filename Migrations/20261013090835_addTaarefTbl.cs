using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addTaarefTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "GlobalOrderDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Taarefs",
                columns: table => new
                {
                    TaarefId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<decimal>(type: "decimal(18,1)", precision: 18, scale: 1, nullable: true),
                    Percentage = table.Column<decimal>(type: "decimal(18,1)", precision: 18, scale: 1, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Business_id = table.Column<int>(type: "int", nullable: false),
                    Service_id = table.Column<int>(type: "int", nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Taarefs", x => x.TaarefId);
                    table.ForeignKey(
                        name: "FK_Taarefs_Businesses_Business_id",
                        column: x => x.Business_id,
                        principalTable: "Businesses",
                        principalColumn: "Business_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Taarefs_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Taarefs_Services_Service_id",
                        column: x => x.Service_id,
                        principalTable: "Services",
                        principalColumn: "Service_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GlobalOrderDetails_CurrencyId",
                table: "GlobalOrderDetails",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Taarefs_Business_id",
                table: "Taarefs",
                column: "Business_id");

            migrationBuilder.CreateIndex(
                name: "IX_Taarefs_CurrencyId",
                table: "Taarefs",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Taarefs_Service_id",
                table: "Taarefs",
                column: "Service_id");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalOrderDetails_Currencies_CurrencyId",
                table: "GlobalOrderDetails",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "CurrencyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GlobalOrderDetails_Currencies_CurrencyId",
                table: "GlobalOrderDetails");

            migrationBuilder.DropTable(
                name: "Taarefs");

            migrationBuilder.DropIndex(
                name: "IX_GlobalOrderDetails_CurrencyId",
                table: "GlobalOrderDetails");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "GlobalOrderDetails");
        }
    }
}
