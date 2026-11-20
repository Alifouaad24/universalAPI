using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addThreeTblsSystem_Business_customerServingGlobalSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GlobalSystems",
                columns: table => new
                {
                    GlobalSystemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GlobalSystemName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GlobalSystemType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalSystems", x => x.GlobalSystemId);
                });

            migrationBuilder.CreateTable(
                name: "Servings",
                columns: table => new
                {
                    ServingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServingName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servings", x => x.ServingId);
                });

            migrationBuilder.CreateTable(
                name: "System_Business_customers",
                columns: table => new
                {
                    System_BusinessId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GlobalSystemId = table.Column<int>(type: "int", nullable: true),
                    Business_id = table.Column<int>(type: "int", nullable: true),
                    GlobalCustomerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_System_Business_customers", x => x.System_BusinessId);
                    table.ForeignKey(
                        name: "FK_System_Business_customers_Businesses_Business_id",
                        column: x => x.Business_id,
                        principalTable: "Businesses",
                        principalColumn: "Business_id");
                    table.ForeignKey(
                        name: "FK_System_Business_customers_GlobalCustomers_GlobalCustomerId",
                        column: x => x.GlobalCustomerId,
                        principalTable: "GlobalCustomers",
                        principalColumn: "GlobalCustomerId");
                    table.ForeignKey(
                        name: "FK_System_Business_customers_GlobalSystems_GlobalSystemId",
                        column: x => x.GlobalSystemId,
                        principalTable: "GlobalSystems",
                        principalColumn: "GlobalSystemId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_System_Business_customers_Business_id",
                table: "System_Business_customers",
                column: "Business_id");

            migrationBuilder.CreateIndex(
                name: "IX_System_Business_customers_GlobalCustomerId",
                table: "System_Business_customers",
                column: "GlobalCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_System_Business_customers_GlobalSystemId",
                table: "System_Business_customers",
                column: "GlobalSystemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Servings");

            migrationBuilder.DropTable(
                name: "System_Business_customers");

            migrationBuilder.DropTable(
                name: "GlobalSystems");
        }
    }
}
