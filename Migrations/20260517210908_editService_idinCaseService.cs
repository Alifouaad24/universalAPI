using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class editService_idinCaseService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Case_Services_Services_ServiceId",
                table: "Case_Services");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "Case_Services",
                newName: "Service_id");

            migrationBuilder.RenameIndex(
                name: "IX_Case_Services_ServiceId",
                table: "Case_Services",
                newName: "IX_Case_Services_Service_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Case_Services_Services_Service_id",
                table: "Case_Services",
                column: "Service_id",
                principalTable: "Services",
                principalColumn: "Service_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Case_Services_Services_Service_id",
                table: "Case_Services");

            migrationBuilder.RenameColumn(
                name: "Service_id",
                table: "Case_Services",
                newName: "ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_Case_Services_Service_id",
                table: "Case_Services",
                newName: "IX_Case_Services_ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Case_Services_Services_ServiceId",
                table: "Case_Services",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Service_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
