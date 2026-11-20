using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addPkTobusSerTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Business_Services",
                table: "Business_Services");

            migrationBuilder.AddColumn<int>(
                name: "Business_ServiceId",
                table: "Business_Services",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<bool>(
                name: "Is_Consumer",
                table: "Business_Services",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Business_Services",
                table: "Business_Services",
                column: "Business_ServiceId");

            migrationBuilder.CreateTable(
                name: "SupplierConsumer",
                columns: table => new
                {
                    SupplierConsumerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierId = table.Column<int>(type: "int", nullable: true),
                    ConsumerBusinessId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierConsumer", x => x.SupplierConsumerId);
                    table.ForeignKey(
                        name: "FK_SupplierConsumer_Businesses_ConsumerBusinessId",
                        column: x => x.ConsumerBusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Business_id");
                    table.ForeignKey(
                        name: "FK_SupplierConsumer_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Business_Services_Business_id_Service_id",
                table: "Business_Services",
                columns: new[] { "Business_id", "Service_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplierConsumer_ConsumerBusinessId",
                table: "SupplierConsumer",
                column: "ConsumerBusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierConsumer_SupplierId",
                table: "SupplierConsumer",
                column: "SupplierId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupplierConsumer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Business_Services",
                table: "Business_Services");

            migrationBuilder.DropIndex(
                name: "IX_Business_Services_Business_id_Service_id",
                table: "Business_Services");

            migrationBuilder.DropColumn(
                name: "Business_ServiceId",
                table: "Business_Services");

            migrationBuilder.DropColumn(
                name: "Is_Consumer",
                table: "Business_Services");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Business_Services",
                table: "Business_Services",
                columns: new[] { "Business_id", "Service_id" });
        }
    }
}
