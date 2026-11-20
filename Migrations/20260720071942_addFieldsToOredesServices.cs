using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addFieldsToOredesServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Cost",
                table: "OredesServices",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "OredesServices",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "OredesServices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Resolved",
                table: "OredesServices",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "paid",
                table: "OredesServices",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "GlobalOrders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "OredesServicesId",
                table: "Case_Service_Notes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CarImage",
                columns: table => new
                {
                    CarImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarInfoTblId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarImage", x => x.CarImageId);
                    table.ForeignKey(
                        name: "FK_CarImage_CarInfoTbls_CarInfoTblId",
                        column: x => x.CarInfoTblId,
                        principalTable: "CarInfoTbls",
                        principalColumn: "CarInfoTblId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Case_Service_Notes_OredesServicesId",
                table: "Case_Service_Notes",
                column: "OredesServicesId");

            migrationBuilder.CreateIndex(
                name: "IX_CarImage_CarInfoTblId",
                table: "CarImage",
                column: "CarInfoTblId");

            migrationBuilder.AddForeignKey(
                name: "FK_Case_Service_Notes_OredesServices_OredesServicesId",
                table: "Case_Service_Notes",
                column: "OredesServicesId",
                principalTable: "OredesServices",
                principalColumn: "OredesServicesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Case_Service_Notes_OredesServices_OredesServicesId",
                table: "Case_Service_Notes");

            migrationBuilder.DropTable(
                name: "CarImage");

            migrationBuilder.DropIndex(
                name: "IX_Case_Service_Notes_OredesServicesId",
                table: "Case_Service_Notes");

            migrationBuilder.DropColumn(
                name: "Cost",
                table: "OredesServices");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "OredesServices");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "OredesServices");

            migrationBuilder.DropColumn(
                name: "Resolved",
                table: "OredesServices");

            migrationBuilder.DropColumn(
                name: "paid",
                table: "OredesServices");

            migrationBuilder.DropColumn(
                name: "OredesServicesId",
                table: "Case_Service_Notes");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "GlobalOrders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
