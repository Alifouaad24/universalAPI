using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addBusiness_idToInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Business_id",
                table: "Inventories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_Business_id",
                table: "Inventories",
                column: "Business_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventories_Businesses_Business_id",
                table: "Inventories",
                column: "Business_id",
                principalTable: "Businesses",
                principalColumn: "Business_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventories_Businesses_Business_id",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_Business_id",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "Business_id",
                table: "Inventories");
        }
    }
}
