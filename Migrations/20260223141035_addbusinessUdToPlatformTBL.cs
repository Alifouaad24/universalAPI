using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addbusinessUdToPlatformTBL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BusinessId",
                table: "Platforms",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Platforms_BusinessId",
                table: "Platforms",
                column: "BusinessId");

            migrationBuilder.AddForeignKey(
                name: "FK_Platforms_Businesses_BusinessId",
                table: "Platforms",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Business_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Platforms_Businesses_BusinessId",
                table: "Platforms");

            migrationBuilder.DropIndex(
                name: "IX_Platforms_BusinessId",
                table: "Platforms");

            migrationBuilder.DropColumn(
                name: "BusinessId",
                table: "Platforms");
        }
    }
}
