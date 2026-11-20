using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class test1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CarInfoTblId",
                table: "GlobalOrders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GlobalOrders_CarInfoTblId",
                table: "GlobalOrders",
                column: "CarInfoTblId");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalOrders_CarInfoTbls_CarInfoTblId",
                table: "GlobalOrders",
                column: "CarInfoTblId",
                principalTable: "CarInfoTbls",
                principalColumn: "CarInfoTblId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GlobalOrders_CarInfoTbls_CarInfoTblId",
                table: "GlobalOrders");

            migrationBuilder.DropIndex(
                name: "IX_GlobalOrders_CarInfoTblId",
                table: "GlobalOrders");

            migrationBuilder.DropColumn(
                name: "CarInfoTblId",
                table: "GlobalOrders");
        }
    }
}
