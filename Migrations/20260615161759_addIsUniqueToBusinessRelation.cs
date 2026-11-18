using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addIsUniqueToBusinessRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BusinessRelations_ProviderBusinessId",
                table: "BusinessRelations");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessRelations_ProviderBusinessId_ConsumerBusinessId",
                table: "BusinessRelations",
                columns: new[] { "ProviderBusinessId", "ConsumerBusinessId" },
                unique: true,
                filter: "[ConsumerBusinessId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BusinessRelations_ProviderBusinessId_ConsumerBusinessId",
                table: "BusinessRelations");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessRelations_ProviderBusinessId",
                table: "BusinessRelations",
                column: "ProviderBusinessId");
        }
    }
}
