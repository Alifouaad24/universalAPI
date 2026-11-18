using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class someUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Service_id",
                table: "GlobalOrders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GlobalOrders_Service_id",
                table: "GlobalOrders",
                column: "Service_id");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalOrders_Services_Service_id",
                table: "GlobalOrders",
                column: "Service_id",
                principalTable: "Services",
                principalColumn: "Service_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GlobalOrders_Services_Service_id",
                table: "GlobalOrders");

            migrationBuilder.DropIndex(
                name: "IX_GlobalOrders_Service_id",
                table: "GlobalOrders");

            migrationBuilder.DropColumn(
                name: "Service_id",
                table: "GlobalOrders");
        }
    }
}
