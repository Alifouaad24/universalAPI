using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class reRelateCarTbls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarModels_CarBrands_CarBrandId",
                table: "CarModels");

            migrationBuilder.DropForeignKey(
                name: "FK_CarYears_CarModels_CarModelId",
                table: "CarYears");

            migrationBuilder.DropIndex(
                name: "IX_CarYears_CarModelId",
                table: "CarYears");

            migrationBuilder.DropColumn(
                name: "CarModelId",
                table: "CarYears");

            migrationBuilder.RenameColumn(
                name: "CarBrandId",
                table: "CarModels",
                newName: "CarYearId");

            migrationBuilder.RenameIndex(
                name: "IX_CarModels_CarBrandId",
                table: "CarModels",
                newName: "IX_CarModels_CarYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarModels_CarYears_CarYearId",
                table: "CarModels",
                column: "CarYearId",
                principalTable: "CarYears",
                principalColumn: "CarYearId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarModels_CarYears_CarYearId",
                table: "CarModels");

            migrationBuilder.RenameColumn(
                name: "CarYearId",
                table: "CarModels",
                newName: "CarBrandId");

            migrationBuilder.RenameIndex(
                name: "IX_CarModels_CarYearId",
                table: "CarModels",
                newName: "IX_CarModels_CarBrandId");

            migrationBuilder.AddColumn<int>(
                name: "CarModelId",
                table: "CarYears",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarYears_CarModelId",
                table: "CarYears",
                column: "CarModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarModels_CarBrands_CarBrandId",
                table: "CarModels",
                column: "CarBrandId",
                principalTable: "CarBrands",
                principalColumn: "CarBrandId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarYears_CarModels_CarModelId",
                table: "CarYears",
                column: "CarModelId",
                principalTable: "CarModels",
                principalColumn: "CarModelId");
        }
    }
}
