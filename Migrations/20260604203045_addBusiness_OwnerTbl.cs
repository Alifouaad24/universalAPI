using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addBusiness_OwnerTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Business_Owners",
                columns: table => new
                {
                    Business_OwnerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GlobalCustomerId = table.Column<int>(type: "int", nullable: true),
                    Business_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Business_Owners", x => x.Business_OwnerId);
                    table.ForeignKey(
                        name: "FK_Business_Owners_Businesses_Business_id",
                        column: x => x.Business_id,
                        principalTable: "Businesses",
                        principalColumn: "Business_id");
                    table.ForeignKey(
                        name: "FK_Business_Owners_GlobalCustomers_GlobalCustomerId",
                        column: x => x.GlobalCustomerId,
                        principalTable: "GlobalCustomers",
                        principalColumn: "GlobalCustomerId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Business_Owners_Business_id",
                table: "Business_Owners",
                column: "Business_id");

            migrationBuilder.CreateIndex(
                name: "IX_Business_Owners_GlobalCustomerId",
                table: "Business_Owners",
                column: "GlobalCustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Business_Owners");
        }
    }
}
