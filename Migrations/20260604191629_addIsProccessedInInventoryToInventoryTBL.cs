using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addIsProccessedInInventoryToInventoryTBL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsProccessedInInventory",
                table: "Inventories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "GlobalSystemName",
                table: "GlobalSystems",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalSystems_GlobalSystemName",
                table: "GlobalSystems",
                column: "GlobalSystemName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GlobalSystems_GlobalSystemName",
                table: "GlobalSystems");

            migrationBuilder.DropColumn(
                name: "IsProccessedInInventory",
                table: "Inventories");

            migrationBuilder.AlterColumn<string>(
                name: "GlobalSystemName",
                table: "GlobalSystems",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
