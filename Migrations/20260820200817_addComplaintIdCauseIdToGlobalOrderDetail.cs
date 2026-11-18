using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addComplaintIdCauseIdToGlobalOrderDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CauseId",
                table: "GlobalOrderDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ComplaintId",
                table: "GlobalOrderDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GlobalOrderDetails_CauseId",
                table: "GlobalOrderDetails",
                column: "CauseId");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalOrderDetails_ComplaintId",
                table: "GlobalOrderDetails",
                column: "ComplaintId");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalOrderDetails_Causes_CauseId",
                table: "GlobalOrderDetails",
                column: "CauseId",
                principalTable: "Causes",
                principalColumn: "CauseId");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalOrderDetails_Complaints_ComplaintId",
                table: "GlobalOrderDetails",
                column: "ComplaintId",
                principalTable: "Complaints",
                principalColumn: "ComplaintId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GlobalOrderDetails_Causes_CauseId",
                table: "GlobalOrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_GlobalOrderDetails_Complaints_ComplaintId",
                table: "GlobalOrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_GlobalOrderDetails_CauseId",
                table: "GlobalOrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_GlobalOrderDetails_ComplaintId",
                table: "GlobalOrderDetails");

            migrationBuilder.DropColumn(
                name: "CauseId",
                table: "GlobalOrderDetails");

            migrationBuilder.DropColumn(
                name: "ComplaintId",
                table: "GlobalOrderDetails");
        }
    }
}
