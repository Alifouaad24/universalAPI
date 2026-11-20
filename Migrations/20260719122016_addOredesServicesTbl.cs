using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addOredesServicesTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarModels_CarYears_CarYearId",
                table: "CarModels");

            migrationBuilder.DropIndex(
                name: "IX_CarModels_CarYearId",
                table: "CarModels");

            migrationBuilder.DropColumn(
                name: "CarYearId",
                table: "CarModels");

            migrationBuilder.AddColumn<int>(
                name: "CarBrandId",
                table: "CarModels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CarYearId",
                table: "CarInfoTbls",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GlobalCustomerId",
                table: "CarInfoTbls",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "CarBrandImgUrl",
                table: "CarBrands",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "GlobalOrders",
                columns: table => new
                {
                    GlobalOrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GlobalCustomerId = table.Column<int>(type: "int", nullable: true),
                    Business_id = table.Column<int>(type: "int", nullable: false),
                    InsertDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Schedule_dt = table.Column<DateOnly>(type: "date", nullable: false),
                    Schedule_time = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalOrders", x => x.GlobalOrderId);
                    table.ForeignKey(
                        name: "FK_GlobalOrders_Businesses_Business_id",
                        column: x => x.Business_id,
                        principalTable: "Businesses",
                        principalColumn: "Business_id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_GlobalOrders_GlobalCustomers_GlobalCustomerId",
                        column: x => x.GlobalCustomerId,
                        principalTable: "GlobalCustomers",
                        principalColumn: "GlobalCustomerId");
                });

            migrationBuilder.CreateTable(
                name: "OredesServices",
                columns: table => new
                {
                    OredesServicesId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceId = table.Column<int>(type: "int", nullable: true),
                    GlobalOrderId = table.Column<int>(type: "int", nullable: true),
                    CarInfoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OredesServices", x => x.OredesServicesId);
                    table.ForeignKey(
                        name: "FK_OredesServices_CarInfoTbls_CarInfoId",
                        column: x => x.CarInfoId,
                        principalTable: "CarInfoTbls",
                        principalColumn: "CarInfoTblId");
                    table.ForeignKey(
                        name: "FK_OredesServices_GlobalOrders_GlobalOrderId",
                        column: x => x.GlobalOrderId,
                        principalTable: "GlobalOrders",
                        principalColumn: "GlobalOrderId");
                    table.ForeignKey(
                        name: "FK_OredesServices_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Service_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarModels_CarBrandId",
                table: "CarModels",
                column: "CarBrandId");

            migrationBuilder.CreateIndex(
                name: "IX_CarInfoTbls_CarYearId",
                table: "CarInfoTbls",
                column: "CarYearId");

            migrationBuilder.CreateIndex(
                name: "IX_CarInfoTbls_GlobalCustomerId",
                table: "CarInfoTbls",
                column: "GlobalCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalOrders_Business_id",
                table: "GlobalOrders",
                column: "Business_id");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalOrders_GlobalCustomerId",
                table: "GlobalOrders",
                column: "GlobalCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_OredesServices_CarInfoId",
                table: "OredesServices",
                column: "CarInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_OredesServices_GlobalOrderId",
                table: "OredesServices",
                column: "GlobalOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OredesServices_ServiceId",
                table: "OredesServices",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarInfoTbls_CarYears_CarYearId",
                table: "CarInfoTbls",
                column: "CarYearId",
                principalTable: "CarYears",
                principalColumn: "CarYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarInfoTbls_GlobalCustomers_GlobalCustomerId",
                table: "CarInfoTbls",
                column: "GlobalCustomerId",
                principalTable: "GlobalCustomers",
                principalColumn: "GlobalCustomerId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_CarModels_CarBrands_CarBrandId",
                table: "CarModels",
                column: "CarBrandId",
                principalTable: "CarBrands",
                principalColumn: "CarBrandId",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarInfoTbls_CarYears_CarYearId",
                table: "CarInfoTbls");

            migrationBuilder.DropForeignKey(
                name: "FK_CarInfoTbls_GlobalCustomers_GlobalCustomerId",
                table: "CarInfoTbls");

            migrationBuilder.DropForeignKey(
                name: "FK_CarModels_CarBrands_CarBrandId",
                table: "CarModels");

            migrationBuilder.DropTable(
                name: "OredesServices");

            migrationBuilder.DropTable(
                name: "GlobalOrders");

            migrationBuilder.DropIndex(
                name: "IX_CarModels_CarBrandId",
                table: "CarModels");

            migrationBuilder.DropIndex(
                name: "IX_CarInfoTbls_CarYearId",
                table: "CarInfoTbls");

            migrationBuilder.DropIndex(
                name: "IX_CarInfoTbls_GlobalCustomerId",
                table: "CarInfoTbls");

            migrationBuilder.DropColumn(
                name: "CarBrandId",
                table: "CarModels");

            migrationBuilder.DropColumn(
                name: "CarYearId",
                table: "CarInfoTbls");

            migrationBuilder.DropColumn(
                name: "GlobalCustomerId",
                table: "CarInfoTbls");

            migrationBuilder.AddColumn<int>(
                name: "CarYearId",
                table: "CarModels",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CarBrandImgUrl",
                table: "CarBrands",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarModels_CarYearId",
                table: "CarModels",
                column: "CarYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarModels_CarYears_CarYearId",
                table: "CarModels",
                column: "CarYearId",
                principalTable: "CarYears",
                principalColumn: "CarYearId");
        }
    }
}
