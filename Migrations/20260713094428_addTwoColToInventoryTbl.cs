using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addTwoColToInventoryTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Qty",
                table: "Inventories",
                newName: "QtyPublished");

            migrationBuilder.AddColumn<int>(
                name: "QtyInInventory",
                table: "Inventories",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QtyInInventory",
                table: "Inventories");

            migrationBuilder.RenameColumn(
                name: "QtyPublished",
                table: "Inventories",
                newName: "Qty");
        }
    }
}
