using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addPublishInventoryTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ColorId",
                table: "OrderItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Inventory_Published_Items",
                columns: table => new
                {
                    Inventory_Published_Items_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Inventory_id = table.Column<int>(type: "int", nullable: false),
                    Platform_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory_Published_Items", x => x.Inventory_Published_Items_Id);
                    table.ForeignKey(
                        name: "FK_Inventory_Published_Items_Inventories_Inventory_id",
                        column: x => x.Inventory_id,
                        principalTable: "Inventories",
                        principalColumn: "Inventory_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Inventory_Published_Items_Platforms_Platform_id",
                        column: x => x.Platform_id,
                        principalTable: "Platforms",
                        principalColumn: "Platform_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ColorId",
                table: "OrderItems",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_Published_Items_Inventory_id",
                table: "Inventory_Published_Items",
                column: "Inventory_id");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_Published_Items_Platform_id",
                table: "Inventory_Published_Items",
                column: "Platform_id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Colors_ColorId",
                table: "OrderItems",
                column: "ColorId",
                principalTable: "Colors",
                principalColumn: "ColorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Colors_ColorId",
                table: "OrderItems");

            migrationBuilder.DropTable(
                name: "Inventory_Published_Items");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_ColorId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ColorId",
                table: "OrderItems");
        }
    }
}
