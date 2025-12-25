using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addService_RouteAnd2Fields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Service_Route",
                table: "Services",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Service_icon",
                table: "Services",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Business_LogoUrl",
                table: "Businesses",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Service_Route",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "Service_icon",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "Business_LogoUrl",
                table: "Businesses");
        }
    }
}
