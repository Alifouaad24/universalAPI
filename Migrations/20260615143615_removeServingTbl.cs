using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class removeServingTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessRelations_Servings_ServingId",
                table: "BusinessRelations");

            migrationBuilder.DropForeignKey(
                name: "FK_GlobalCustomers_Businesses_Business_id",
                table: "GlobalCustomers");

            migrationBuilder.DropTable(
                name: "Servings");

            migrationBuilder.DropIndex(
                name: "IX_GlobalCustomers_Business_id",
                table: "GlobalCustomers");

            migrationBuilder.DropIndex(
                name: "IX_BusinessRelations_ServingId",
                table: "BusinessRelations");

            migrationBuilder.DropColumn(
                name: "Business_id",
                table: "GlobalCustomers");

            migrationBuilder.DropColumn(
                name: "ServingId",
                table: "BusinessRelations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Business_id",
                table: "GlobalCustomers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServingId",
                table: "BusinessRelations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Servings",
                columns: table => new
                {
                    ServingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServingName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servings", x => x.ServingId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GlobalCustomers_Business_id",
                table: "GlobalCustomers",
                column: "Business_id");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessRelations_ServingId",
                table: "BusinessRelations",
                column: "ServingId");

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessRelations_Servings_ServingId",
                table: "BusinessRelations",
                column: "ServingId",
                principalTable: "Servings",
                principalColumn: "ServingId");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalCustomers_Businesses_Business_id",
                table: "GlobalCustomers",
                column: "Business_id",
                principalTable: "Businesses",
                principalColumn: "Business_id");
        }
    }
}
