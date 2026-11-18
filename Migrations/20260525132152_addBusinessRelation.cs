using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addBusinessRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusinessRelations",
                columns: table => new
                {
                    BusinessRelationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderBusinessId = table.Column<int>(type: "int", nullable: false),
                    ConsumerBusinessId = table.Column<int>(type: "int", nullable: true),
                    GlobalCustomerId = table.Column<int>(type: "int", nullable: true),
                    ServingId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessRelations", x => x.BusinessRelationId);
                    table.ForeignKey(
                        name: "FK_BusinessRelations_Businesses_ConsumerBusinessId",
                        column: x => x.ConsumerBusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Business_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BusinessRelations_Businesses_ProviderBusinessId",
                        column: x => x.ProviderBusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Business_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BusinessRelations_GlobalCustomers_GlobalCustomerId",
                        column: x => x.GlobalCustomerId,
                        principalTable: "GlobalCustomers",
                        principalColumn: "GlobalCustomerId");
                    table.ForeignKey(
                        name: "FK_BusinessRelations_Servings_ServingId",
                        column: x => x.ServingId,
                        principalTable: "Servings",
                        principalColumn: "ServingId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessRelations_ConsumerBusinessId",
                table: "BusinessRelations",
                column: "ConsumerBusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessRelations_GlobalCustomerId",
                table: "BusinessRelations",
                column: "GlobalCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessRelations_ProviderBusinessId",
                table: "BusinessRelations",
                column: "ProviderBusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessRelations_ServingId",
                table: "BusinessRelations",
                column: "ServingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessRelations");
        }
    }
}
