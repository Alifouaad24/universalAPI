using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addShippingListsTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Taarefs_Businesses_Business_id",
                table: "Taarefs");

            migrationBuilder.DropForeignKey(
                name: "FK_Taarefs_Currencies_CurrencyId",
                table: "Taarefs");

            migrationBuilder.DropForeignKey(
                name: "FK_Taarefs_Services_Service_id",
                table: "Taarefs");

            migrationBuilder.DropIndex(
                name: "IX_Taarefs_CurrencyId",
                table: "Taarefs");

            migrationBuilder.DropIndex(
                name: "IX_Taarefs_Service_id",
                table: "Taarefs");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Taarefs");

            migrationBuilder.DropColumn(
                name: "Percentage",
                table: "Taarefs");

            migrationBuilder.DropColumn(
                name: "Service_id",
                table: "Taarefs");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "Taarefs");

            migrationBuilder.RenameColumn(
                name: "CurrencyId",
                table: "Taarefs",
                newName: "TariffType");

            migrationBuilder.RenameColumn(
                name: "Business_id",
                table: "Taarefs",
                newName: "Platform_id");

            migrationBuilder.RenameIndex(
                name: "IX_Taarefs_Business_id",
                table: "Taarefs",
                newName: "IX_Taarefs_Platform_id");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Taarefs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Account",
                table: "Platforms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Ads",
                table: "Platforms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Chat",
                table: "Platforms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Post",
                table: "Platforms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Scrape",
                table: "Platforms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Selling",
                table: "Platforms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "PlatformAccounts",
                columns: table => new
                {
                    PlatformAccountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlatformId = table.Column<int>(type: "int", nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformAccounts", x => x.PlatformAccountId);
                    table.ForeignKey(
                        name: "FK_PlatformAccounts_Platforms_PlatformId",
                        column: x => x.PlatformId,
                        principalTable: "Platforms",
                        principalColumn: "Platform_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShippingLists",
                columns: table => new
                {
                    ShippingListId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrencyId = table.Column<int>(type: "int", nullable: true),
                    FromCountryId = table.Column<int>(type: "int", nullable: true),
                    ToCountryId = table.Column<int>(type: "int", nullable: true),
                    ShippingTypeId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingLists", x => x.ShippingListId);
                    table.ForeignKey(
                        name: "FK_ShippingLists_Countries_FromCountryId",
                        column: x => x.FromCountryId,
                        principalTable: "Countries",
                        principalColumn: "CountryId");
                    table.ForeignKey(
                        name: "FK_ShippingLists_Countries_ToCountryId",
                        column: x => x.ToCountryId,
                        principalTable: "Countries",
                        principalColumn: "CountryId");
                    table.ForeignKey(
                        name: "FK_ShippingLists_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId");
                    table.ForeignKey(
                        name: "FK_ShippingLists_ShippingTypes_ShippingTypeId",
                        column: x => x.ShippingTypeId,
                        principalTable: "ShippingTypes",
                        principalColumn: "ShippingTypeId");
                });

            migrationBuilder.CreateTable(
                name: "TariffDetails",
                columns: table => new
                {
                    TariffDetailsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaarefId = table.Column<int>(type: "int", nullable: true),
                    Service_id = table.Column<int>(type: "int", nullable: true),
                    ShippingListId = table.Column<int>(type: "int", nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: true),
                    CurrencyId = table.Column<int>(type: "int", nullable: true),
                    UnitId = table.Column<int>(type: "int", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,1)", precision: 18, scale: 1, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TariffDetails", x => x.TariffDetailsId);
                    table.ForeignKey(
                        name: "FK_TariffDetails_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId");
                    table.ForeignKey(
                        name: "FK_TariffDetails_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId");
                    table.ForeignKey(
                        name: "FK_TariffDetails_Services_Service_id",
                        column: x => x.Service_id,
                        principalTable: "Services",
                        principalColumn: "Service_id");
                    table.ForeignKey(
                        name: "FK_TariffDetails_ShippingLists_ShippingListId",
                        column: x => x.ShippingListId,
                        principalTable: "ShippingLists",
                        principalColumn: "ShippingListId");
                    table.ForeignKey(
                        name: "FK_TariffDetails_Taarefs_TaarefId",
                        column: x => x.TaarefId,
                        principalTable: "Taarefs",
                        principalColumn: "TaarefId");
                    table.ForeignKey(
                        name: "FK_TariffDetails_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "UnitId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlatformAccounts_PlatformId",
                table: "PlatformAccounts",
                column: "PlatformId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingLists_CurrencyId",
                table: "ShippingLists",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingLists_FromCountryId",
                table: "ShippingLists",
                column: "FromCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingLists_ShippingTypeId",
                table: "ShippingLists",
                column: "ShippingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingLists_ToCountryId",
                table: "ShippingLists",
                column: "ToCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_TariffDetails_CurrencyId",
                table: "TariffDetails",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_TariffDetails_ItemId",
                table: "TariffDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_TariffDetails_Service_id",
                table: "TariffDetails",
                column: "Service_id");

            migrationBuilder.CreateIndex(
                name: "IX_TariffDetails_ShippingListId",
                table: "TariffDetails",
                column: "ShippingListId");

            migrationBuilder.CreateIndex(
                name: "IX_TariffDetails_TaarefId",
                table: "TariffDetails",
                column: "TaarefId");

            migrationBuilder.CreateIndex(
                name: "IX_TariffDetails_UnitId",
                table: "TariffDetails",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_Taarefs_Platforms_Platform_id",
                table: "Taarefs",
                column: "Platform_id",
                principalTable: "Platforms",
                principalColumn: "Platform_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Taarefs_Platforms_Platform_id",
                table: "Taarefs");

            migrationBuilder.DropTable(
                name: "PlatformAccounts");

            migrationBuilder.DropTable(
                name: "TariffDetails");

            migrationBuilder.DropTable(
                name: "ShippingLists");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Taarefs");

            migrationBuilder.DropColumn(
                name: "Account",
                table: "Platforms");

            migrationBuilder.DropColumn(
                name: "Ads",
                table: "Platforms");

            migrationBuilder.DropColumn(
                name: "Chat",
                table: "Platforms");

            migrationBuilder.DropColumn(
                name: "Post",
                table: "Platforms");

            migrationBuilder.DropColumn(
                name: "Scrape",
                table: "Platforms");

            migrationBuilder.DropColumn(
                name: "Selling",
                table: "Platforms");

            migrationBuilder.RenameColumn(
                name: "TariffType",
                table: "Taarefs",
                newName: "CurrencyId");

            migrationBuilder.RenameColumn(
                name: "Platform_id",
                table: "Taarefs",
                newName: "Business_id");

            migrationBuilder.RenameIndex(
                name: "IX_Taarefs_Platform_id",
                table: "Taarefs",
                newName: "IX_Taarefs_Business_id");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Taarefs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Percentage",
                table: "Taarefs",
                type: "decimal(18,1)",
                precision: 18,
                scale: 1,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Service_id",
                table: "Taarefs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Value",
                table: "Taarefs",
                type: "decimal(18,1)",
                precision: 18,
                scale: 1,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Taarefs_CurrencyId",
                table: "Taarefs",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Taarefs_Service_id",
                table: "Taarefs",
                column: "Service_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Taarefs_Businesses_Business_id",
                table: "Taarefs",
                column: "Business_id",
                principalTable: "Businesses",
                principalColumn: "Business_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Taarefs_Currencies_CurrencyId",
                table: "Taarefs",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Taarefs_Services_Service_id",
                table: "Taarefs",
                column: "Service_id",
                principalTable: "Services",
                principalColumn: "Service_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
