using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addBuseness_CustomerTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusinessName",
                table: "Cases");

            migrationBuilder.CreateTable(
                name: "Buseness_Customers",
                columns: table => new
                {
                    Buseness_CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GlobalCustomerId = table.Column<int>(type: "int", nullable: true),
                    Business_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buseness_Customers", x => x.Buseness_CustomerId);
                    table.ForeignKey(
                        name: "FK_Buseness_Customers_Businesses_Business_id",
                        column: x => x.Business_id,
                        principalTable: "Businesses",
                        principalColumn: "Business_id");
                    table.ForeignKey(
                        name: "FK_Buseness_Customers_GlobalCustomers_GlobalCustomerId",
                        column: x => x.GlobalCustomerId,
                        principalTable: "GlobalCustomers",
                        principalColumn: "GlobalCustomerId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Buseness_Customers_Business_id",
                table: "Buseness_Customers",
                column: "Business_id");

            migrationBuilder.CreateIndex(
                name: "IX_Buseness_Customers_GlobalCustomerId",
                table: "Buseness_Customers",
                column: "GlobalCustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Buseness_Customers");

            migrationBuilder.AddColumn<string>(
                name: "BusinessName",
                table: "Cases",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
