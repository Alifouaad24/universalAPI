using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addCase_ServiceTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Case_Services",
                columns: table => new
                {
                    Case_ServiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaseID = table.Column<int>(type: "int", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    Resolved = table.Column<bool>(type: "bit", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    paid = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Case_Services", x => x.Case_ServiceId);
                    table.ForeignKey(
                        name: "FK_Case_Services_Cases_CaseID",
                        column: x => x.CaseID,
                        principalTable: "Cases",
                        principalColumn: "CaseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Case_Services_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Service_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Case_Service_Notes",
                columns: table => new
                {
                    Case_ServiceNotesId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Case_ServiceId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Case_Service_Notes", x => x.Case_ServiceNotesId);
                    table.ForeignKey(
                        name: "FK_Case_Service_Notes_Case_Services_Case_ServiceId",
                        column: x => x.Case_ServiceId,
                        principalTable: "Case_Services",
                        principalColumn: "Case_ServiceId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Case_Service_Notes_Case_ServiceId",
                table: "Case_Service_Notes",
                column: "Case_ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Case_Services_CaseID",
                table: "Case_Services",
                column: "CaseID");

            migrationBuilder.CreateIndex(
                name: "IX_Case_Services_ServiceId",
                table: "Case_Services",
                column: "ServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Case_Service_Notes");

            migrationBuilder.DropTable(
                name: "Case_Services");
        }
    }
}
