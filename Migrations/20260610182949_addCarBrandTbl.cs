using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addCarBrandTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CarBrandId",
                table: "CarInfoTbls",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CarBrands",
                columns: table => new
                {
                    CarBrandId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarBrandName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CarBrandImgUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarBrands", x => x.CarBrandId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarInfoTbls_CarBrandId",
                table: "CarInfoTbls",
                column: "CarBrandId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarInfoTbls_CarBrands_CarBrandId",
                table: "CarInfoTbls",
                column: "CarBrandId",
                principalTable: "CarBrands",
                principalColumn: "CarBrandId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarInfoTbls_CarBrands_CarBrandId",
                table: "CarInfoTbls");

            migrationBuilder.DropTable(
                name: "CarBrands");

            migrationBuilder.DropIndex(
                name: "IX_CarInfoTbls_CarBrandId",
                table: "CarInfoTbls");

            migrationBuilder.DropColumn(
                name: "CarBrandId",
                table: "CarInfoTbls");
        }
    }
}
