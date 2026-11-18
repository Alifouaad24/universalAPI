using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addddg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupplierConsumer");

            migrationBuilder.DropTable(
                name: "SupplierPlatforms");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.CreateTable(
                name: "Business_Service_Platforms",
                columns: table => new
                {
                    Business_Service_PlatformId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Business_ServiceId = table.Column<int>(type: "int", nullable: true),
                    Platform_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Business_Service_Platforms", x => x.Business_Service_PlatformId);
                    table.ForeignKey(
                        name: "FK_Business_Service_Platforms_Business_Services_Business_ServiceId",
                        column: x => x.Business_ServiceId,
                        principalTable: "Business_Services",
                        principalColumn: "Business_ServiceId");
                    table.ForeignKey(
                        name: "FK_Business_Service_Platforms_Platforms_Platform_id",
                        column: x => x.Platform_id,
                        principalTable: "Platforms",
                        principalColumn: "Platform_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Business_Service_Platforms_Business_ServiceId",
                table: "Business_Service_Platforms",
                column: "Business_ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Business_Service_Platforms_Platform_id",
                table: "Business_Service_Platforms",
                column: "Platform_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Business_Service_Platforms");

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    SupplierId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Business_id = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateOnly>(type: "date", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.SupplierId);
                    table.ForeignKey(
                        name: "FK_Suppliers_Businesses_Business_id",
                        column: x => x.Business_id,
                        principalTable: "Businesses",
                        principalColumn: "Business_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SupplierConsumer",
                columns: table => new
                {
                    SupplierConsumerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConsumerBusinessId = table.Column<int>(type: "int", nullable: true),
                    SupplierId = table.Column<int>(type: "int", nullable: true),
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

            migrationBuilder.CreateTable(
                name: "SupplierPlatforms",
                columns: table => new
                {
                    SupplierPlatformId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Platform_id = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierPlatforms", x => x.SupplierPlatformId);
                    table.ForeignKey(
                        name: "FK_SupplierPlatforms_Platforms_Platform_id",
                        column: x => x.Platform_id,
                        principalTable: "Platforms",
                        principalColumn: "Platform_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupplierPlatforms_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierConsumer_ConsumerBusinessId",
                table: "SupplierConsumer",
                column: "ConsumerBusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierConsumer_SupplierId",
                table: "SupplierConsumer",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPlatforms_Platform_id",
                table: "SupplierPlatforms",
                column: "Platform_id");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPlatforms_SupplierId",
                table: "SupplierPlatforms",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_Business_id",
                table: "Suppliers",
                column: "Business_id");
        }
    }
}
