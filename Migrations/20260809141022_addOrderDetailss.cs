using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addOrderDetailss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GlobalOrderDetailId",
                table: "GlobalOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GlobalOrderDetailId",
                table: "Case_Service_Notes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GlobalOrderDetail",
                columns: table => new
                {
                    GlobalOrderDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceId = table.Column<int>(type: "int", nullable: true),
                    GlobalOrderId = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    paid = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalOrderDetail", x => x.GlobalOrderDetailId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GlobalOrders_GlobalOrderDetailId",
                table: "GlobalOrders",
                column: "GlobalOrderDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_Case_Service_Notes_GlobalOrderDetailId",
                table: "Case_Service_Notes",
                column: "GlobalOrderDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_Case_Service_Notes_GlobalOrderDetail_GlobalOrderDetailId",
                table: "Case_Service_Notes",
                column: "GlobalOrderDetailId",
                principalTable: "GlobalOrderDetail",
                principalColumn: "GlobalOrderDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalOrders_GlobalOrderDetail_GlobalOrderDetailId",
                table: "GlobalOrders",
                column: "GlobalOrderDetailId",
                principalTable: "GlobalOrderDetail",
                principalColumn: "GlobalOrderDetailId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Case_Service_Notes_GlobalOrderDetail_GlobalOrderDetailId",
                table: "Case_Service_Notes");

            migrationBuilder.DropForeignKey(
                name: "FK_GlobalOrders_GlobalOrderDetail_GlobalOrderDetailId",
                table: "GlobalOrders");

            migrationBuilder.DropTable(
                name: "GlobalOrderDetail");

            migrationBuilder.DropIndex(
                name: "IX_GlobalOrders_GlobalOrderDetailId",
                table: "GlobalOrders");

            migrationBuilder.DropIndex(
                name: "IX_Case_Service_Notes_GlobalOrderDetailId",
                table: "Case_Service_Notes");

            migrationBuilder.DropColumn(
                name: "GlobalOrderDetailId",
                table: "GlobalOrders");

            migrationBuilder.DropColumn(
                name: "GlobalOrderDetailId",
                table: "Case_Service_Notes");
        }
    }
}
