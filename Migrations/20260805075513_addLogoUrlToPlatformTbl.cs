using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addLogoUrlToPlatformTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Business_Service_Platforms_Business_Services_Business_ServiceId",
                table: "Business_Service_Platforms");

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "Platforms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Business_ServiceId",
                table: "Business_Service_Platforms",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Business_Service_Platforms_Business_Services_Business_ServiceId",
                table: "Business_Service_Platforms",
                column: "Business_ServiceId",
                principalTable: "Business_Services",
                principalColumn: "Business_ServiceId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Business_Service_Platforms_Business_Services_Business_ServiceId",
                table: "Business_Service_Platforms");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "Platforms");

            migrationBuilder.AlterColumn<int>(
                name: "Business_ServiceId",
                table: "Business_Service_Platforms",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Business_Service_Platforms_Business_Services_Business_ServiceId",
                table: "Business_Service_Platforms",
                column: "Business_ServiceId",
                principalTable: "Business_Services",
                principalColumn: "Business_ServiceId");
        }
    }
}
