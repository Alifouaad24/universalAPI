using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addInsertByToItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Platforms_Businesses_Business_id",
                table: "Platforms");

            migrationBuilder.DropIndex(
                name: "IX_Platforms_Business_id",
                table: "Platforms");

            migrationBuilder.DropColumn(
                name: "Business_id",
                table: "Platforms");

            migrationBuilder.AddColumn<string>(
                name: "InsertBy",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InsertBy",
                table: "Items");

            migrationBuilder.AddColumn<int>(
                name: "Business_id",
                table: "Platforms",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Platforms_Business_id",
                table: "Platforms",
                column: "Business_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Platforms_Businesses_Business_id",
                table: "Platforms",
                column: "Business_id",
                principalTable: "Businesses",
                principalColumn: "Business_id");
        }
    }
}
