using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addServiceStatusIdToOrderDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServiceStatusId",
                table: "GlobalOrderDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GlobalOrderDetails_ServiceStatusId",
                table: "GlobalOrderDetails",
                column: "ServiceStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalOrderDetails_UniversalOrderStatus_ServiceStatusId",
                table: "GlobalOrderDetails",
                column: "ServiceStatusId",
                principalTable: "UniversalOrderStatus",
                principalColumn: "OrderStatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GlobalOrderDetails_UniversalOrderStatus_ServiceStatusId",
                table: "GlobalOrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_GlobalOrderDetails_ServiceStatusId",
                table: "GlobalOrderDetails");

            migrationBuilder.DropColumn(
                name: "ServiceStatusId",
                table: "GlobalOrderDetails");
        }
    }
}
