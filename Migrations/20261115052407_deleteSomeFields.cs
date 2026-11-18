using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class deleteSomeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Account",
                table: "BusinessPlatforms");

            migrationBuilder.DropColumn(
                name: "Ads",
                table: "BusinessPlatforms");

            migrationBuilder.DropColumn(
                name: "Chat",
                table: "BusinessPlatforms");

            migrationBuilder.DropColumn(
                name: "Post",
                table: "BusinessPlatforms");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Account",
                table: "BusinessPlatforms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Ads",
                table: "BusinessPlatforms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Chat",
                table: "BusinessPlatforms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Post",
                table: "BusinessPlatforms",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
