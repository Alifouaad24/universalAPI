using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addNotFoundColToInvTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventories_Sizes_Size_id1",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_Size_id1",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "Size_id1",
                table: "Inventories");

            migrationBuilder.AddColumn<bool>(
                name: "NotFound",
                table: "Inventories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_Size_id",
                table: "Inventories",
                column: "Size_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventories_Sizes_Size_id",
                table: "Inventories",
                column: "Size_id",
                principalTable: "Sizes",
                principalColumn: "Size_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventories_Sizes_Size_id",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_Size_id",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "NotFound",
                table: "Inventories");

            migrationBuilder.AddColumn<int>(
                name: "Size_id1",
                table: "Inventories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_Size_id1",
                table: "Inventories",
                column: "Size_id1");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventories_Sizes_Size_id1",
                table: "Inventories",
                column: "Size_id1",
                principalTable: "Sizes",
                principalColumn: "Size_id");
        }
    }
}
