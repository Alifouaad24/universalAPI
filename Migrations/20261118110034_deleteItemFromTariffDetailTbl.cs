using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class deleteItemFromTariffDetailTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TariffDetails_Items_ItemId",
                table: "TariffDetails");

            migrationBuilder.DropIndex(
                name: "IX_TariffDetails_ItemId",
                table: "TariffDetails");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "TariffDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ItemId",
                table: "TariffDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TariffDetails_ItemId",
                table: "TariffDetails",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_TariffDetails_Items_ItemId",
                table: "TariffDetails",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "ItemId");
        }
    }
}
