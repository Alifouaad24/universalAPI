using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addSupplierTblV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    SupplierId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderId = table.Column<int>(type: "int", nullable: true),
                    ConsumerId = table.Column<int>(type: "int", nullable: true),
                    ServiceId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.SupplierId);
                    table.ForeignKey(
                        name: "FK_Suppliers_Businesses_ConsumerId",
                        column: x => x.ConsumerId,
                        principalTable: "Businesses",
                        principalColumn: "Business_id");
                    table.ForeignKey(
                        name: "FK_Suppliers_Businesses_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Businesses",
                        principalColumn: "Business_id");
                    table.ForeignKey(
                        name: "FK_Suppliers_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Service_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_ConsumerId",
                table: "Suppliers",
                column: "ConsumerId");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_ProviderId",
                table: "Suppliers",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_ServiceId",
                table: "Suppliers",
                column: "ServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Suppliers");
        }
    }
}
