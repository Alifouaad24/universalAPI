using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class bindBusinessWithCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Business_id",
                table: "Categories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Business_id",
                table: "Categories",
                column: "Business_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Businesses_Business_id",
                table: "Categories",
                column: "Business_id",
                principalTable: "Businesses",
                principalColumn: "Business_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Businesses_Business_id",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_Business_id",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Business_id",
                table: "Categories");
        }
    }
}
