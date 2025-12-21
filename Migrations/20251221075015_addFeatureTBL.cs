using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addFeatureTBL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_Activiities_Activity_id",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_Activity_id",
                table: "Services");

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Services",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Activity_Services",
                columns: table => new
                {
                    Activity_id = table.Column<int>(type: "int", nullable: false),
                    Service_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activity_Services", x => new { x.Activity_id, x.Service_id });
                    table.ForeignKey(
                        name: "FK_Activity_Services_Activiities_Activity_id",
                        column: x => x.Activity_id,
                        principalTable: "Activiities",
                        principalColumn: "Activity_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Activity_Services_Services_Service_id",
                        column: x => x.Service_id,
                        principalTable: "Services",
                        principalColumn: "Service_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Business_Services",
                columns: table => new
                {
                    Business_id = table.Column<int>(type: "int", nullable: false),
                    Service_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Business_Services", x => new { x.Business_id, x.Service_id });
                    table.ForeignKey(
                        name: "FK_Business_Services_Businesses_Business_id",
                        column: x => x.Business_id,
                        principalTable: "Businesses",
                        principalColumn: "Business_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Business_Services_Services_Service_id",
                        column: x => x.Service_id,
                        principalTable: "Services",
                        principalColumn: "Service_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Features",
                columns: table => new
                {
                    FeatureId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Service_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Features", x => x.FeatureId);
                    table.ForeignKey(
                        name: "FK_Features_Services_Service_id",
                        column: x => x.Service_id,
                        principalTable: "Services",
                        principalColumn: "Service_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activity_Services_Service_id",
                table: "Activity_Services",
                column: "Service_id");

            migrationBuilder.CreateIndex(
                name: "IX_Business_Services_Service_id",
                table: "Business_Services",
                column: "Service_id");

            migrationBuilder.CreateIndex(
                name: "IX_Features_Service_id",
                table: "Features",
                column: "Service_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Activity_Services");

            migrationBuilder.DropTable(
                name: "Business_Services");

            migrationBuilder.DropTable(
                name: "Features");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Services");

            migrationBuilder.CreateIndex(
                name: "IX_Services_Activity_id",
                table: "Services",
                column: "Activity_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Activiities_Activity_id",
                table: "Services",
                column: "Activity_id",
                principalTable: "Activiities",
                principalColumn: "Activity_id");
        }
    }
}
