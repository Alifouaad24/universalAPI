using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class editinitem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Platforms_PlatformId",
                table: "Items");

            migrationBuilder.AlterColumn<int>(
                name: "PlatformId",
                table: "Items",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Platforms_PlatformId",
                table: "Items",
                column: "PlatformId",
                principalTable: "Platforms",
                principalColumn: "Platform_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Platforms_PlatformId",
                table: "Items");

            migrationBuilder.AlterColumn<int>(
                name: "PlatformId",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Platforms_PlatformId",
                table: "Items",
                column: "PlatformId",
                principalTable: "Platforms",
                principalColumn: "Platform_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
