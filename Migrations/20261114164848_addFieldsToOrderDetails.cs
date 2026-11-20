using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addFieldsToOrderDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ColorId",
                table: "GlobalOrderDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "GlobalOrderDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Size_id",
                table: "GlobalOrderDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GlobalOrderDetails_ColorId",
                table: "GlobalOrderDetails",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalOrderDetails_Size_id",
                table: "GlobalOrderDetails",
                column: "Size_id");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalOrderDetails_Colors_ColorId",
                table: "GlobalOrderDetails",
                column: "ColorId",
                principalTable: "Colors",
                principalColumn: "ColorId");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalOrderDetails_Sizes_Size_id",
                table: "GlobalOrderDetails",
                column: "Size_id",
                principalTable: "Sizes",
                principalColumn: "Size_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GlobalOrderDetails_Colors_ColorId",
                table: "GlobalOrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_GlobalOrderDetails_Sizes_Size_id",
                table: "GlobalOrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_GlobalOrderDetails_ColorId",
                table: "GlobalOrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_GlobalOrderDetails_Size_id",
                table: "GlobalOrderDetails");

            migrationBuilder.DropColumn(
                name: "ColorId",
                table: "GlobalOrderDetails");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "GlobalOrderDetails");

            migrationBuilder.DropColumn(
                name: "Size_id",
                table: "GlobalOrderDetails");
        }
    }
}
