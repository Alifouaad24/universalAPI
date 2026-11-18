using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class checkMissiedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Brand",
                table: "Items");

            migrationBuilder.AddColumn<int>(
                name: "CarBrandId",
                table: "Items",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserImages_ItemId",
                table: "UserImages",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_CarBrandId",
                table: "Items",
                column: "CarBrandId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_CarBrands_CarBrandId",
                table: "Items",
                column: "CarBrandId",
                principalTable: "CarBrands",
                principalColumn: "CarBrandId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserImages_Items_ItemId",
                table: "UserImages",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "ItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_CarBrands_CarBrandId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_UserImages_Items_ItemId",
                table: "UserImages");

            migrationBuilder.DropIndex(
                name: "IX_UserImages_ItemId",
                table: "UserImages");

            migrationBuilder.DropIndex(
                name: "IX_Items_CarBrandId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "CarBrandId",
                table: "Items");

            migrationBuilder.AddColumn<string>(
                name: "Brand",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
