using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addSomeFieldsToPlatformAndSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingLists_Currencies_CurrencyId",
                table: "ShippingLists");

            migrationBuilder.DropIndex(
                name: "IX_ShippingLists_CurrencyId",
                table: "ShippingLists");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "ShippingLists");

            migrationBuilder.RenameColumn(
                name: "AccountName",
                table: "PlatformAccounts",
                newName: "UserId");

            migrationBuilder.AddColumn<string>(
                name: "Account",
                table: "PlatformAccounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "BusinessGroupId",
                table: "GlobalSystems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Account",
                table: "BusinessPlatforms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Ads",
                table: "BusinessPlatforms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Chat",
                table: "BusinessPlatforms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Post",
                table: "BusinessPlatforms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_GlobalSystems_BusinessGroupId",
                table: "GlobalSystems",
                column: "BusinessGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalSystems_BusinessGroups_BusinessGroupId",
                table: "GlobalSystems",
                column: "BusinessGroupId",
                principalTable: "BusinessGroups",
                principalColumn: "BusinessGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GlobalSystems_BusinessGroups_BusinessGroupId",
                table: "GlobalSystems");

            migrationBuilder.DropIndex(
                name: "IX_GlobalSystems_BusinessGroupId",
                table: "GlobalSystems");

            migrationBuilder.DropColumn(
                name: "Account",
                table: "PlatformAccounts");

            migrationBuilder.DropColumn(
                name: "BusinessGroupId",
                table: "GlobalSystems");

            migrationBuilder.DropColumn(
                name: "Account",
                table: "BusinessPlatforms");

            migrationBuilder.DropColumn(
                name: "Ads",
                table: "BusinessPlatforms");

            migrationBuilder.DropColumn(
                name: "Chat",
                table: "BusinessPlatforms");

            migrationBuilder.DropColumn(
                name: "Post",
                table: "BusinessPlatforms");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "PlatformAccounts",
                newName: "AccountName");

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "ShippingLists",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingLists_CurrencyId",
                table: "ShippingLists",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingLists_Currencies_CurrencyId",
                table: "ShippingLists",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "CurrencyId");
        }
    }
}
