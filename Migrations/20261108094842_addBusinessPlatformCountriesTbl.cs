using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addBusinessPlatformCountriesTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusinessPlatformCountriess",
                columns: table => new
                {
                    BusinessPlatformCountriesId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusinessId = table.Column<int>(type: "int", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    PlatformId = table.Column<int>(type: "int", nullable: true),
                    ShippingTypeId = table.Column<int>(type: "int", nullable: true),
                    TaarefId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessPlatformCountriess", x => x.BusinessPlatformCountriesId);
                    table.ForeignKey(
                        name: "FK_BusinessPlatformCountriess_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Business_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BusinessPlatformCountriess_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BusinessPlatformCountriess_Platforms_PlatformId",
                        column: x => x.PlatformId,
                        principalTable: "Platforms",
                        principalColumn: "Platform_id");
                    table.ForeignKey(
                        name: "FK_BusinessPlatformCountriess_ShippingTypes_ShippingTypeId",
                        column: x => x.ShippingTypeId,
                        principalTable: "ShippingTypes",
                        principalColumn: "ShippingTypeId");
                    table.ForeignKey(
                        name: "FK_BusinessPlatformCountriess_Taarefs_TaarefId",
                        column: x => x.TaarefId,
                        principalTable: "Taarefs",
                        principalColumn: "TaarefId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessPlatformCountriess_BusinessId_CountryId_PlatformId_ShippingTypeId",
                table: "BusinessPlatformCountriess",
                columns: new[] { "BusinessId", "CountryId", "PlatformId", "ShippingTypeId" },
                unique: true,
                filter: "[PlatformId] IS NOT NULL AND [ShippingTypeId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessPlatformCountriess_CountryId",
                table: "BusinessPlatformCountriess",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessPlatformCountriess_PlatformId",
                table: "BusinessPlatformCountriess",
                column: "PlatformId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessPlatformCountriess_ShippingTypeId",
                table: "BusinessPlatformCountriess",
                column: "ShippingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessPlatformCountriess_TaarefId",
                table: "BusinessPlatformCountriess",
                column: "TaarefId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessPlatformCountriess");
        }
    }
}
