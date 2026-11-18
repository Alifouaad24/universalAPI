using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addUnitToOrderdetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UnitId",
                table: "GlobalOrderDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitValue",
                table: "GlobalOrderDetails",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GlobalOrderDetails_UnitId",
                table: "GlobalOrderDetails",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalOrderDetails_Units_UnitId",
                table: "GlobalOrderDetails",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "UnitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GlobalOrderDetails_Units_UnitId",
                table: "GlobalOrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_GlobalOrderDetails_UnitId",
                table: "GlobalOrderDetails");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "GlobalOrderDetails");

            migrationBuilder.DropColumn(
                name: "UnitValue",
                table: "GlobalOrderDetails");
        }
    }
}
