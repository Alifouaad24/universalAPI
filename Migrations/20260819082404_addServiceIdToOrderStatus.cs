using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addServiceIdToOrderStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Service_id",
                table: "UniversalOrderStatus",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UniversalOrderStatus_Service_id",
                table: "UniversalOrderStatus",
                column: "Service_id");

            migrationBuilder.AddForeignKey(
                name: "FK_UniversalOrderStatus_Services_Service_id",
                table: "UniversalOrderStatus",
                column: "Service_id",
                principalTable: "Services",
                principalColumn: "Service_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UniversalOrderStatus_Services_Service_id",
                table: "UniversalOrderStatus");

            migrationBuilder.DropIndex(
                name: "IX_UniversalOrderStatus_Service_id",
                table: "UniversalOrderStatus");

            migrationBuilder.DropColumn(
                name: "Service_id",
                table: "UniversalOrderStatus");
        }
    }
}
