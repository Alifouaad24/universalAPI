using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class ReDesignFeatureTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Features_Services_Service_id",
                table: "Features");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Features");

            migrationBuilder.RenameColumn(
                name: "Service_id",
                table: "Features",
                newName: "GlobalSystemId");

            migrationBuilder.RenameIndex(
                name: "IX_Features_Service_id",
                table: "Features",
                newName: "IX_Features_GlobalSystemId");

            migrationBuilder.AddColumn<string>(
                name: "Body",
                table: "Features",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "Features",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Features",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Features",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Features_GlobalSystems_GlobalSystemId",
                table: "Features",
                column: "GlobalSystemId",
                principalTable: "GlobalSystems",
                principalColumn: "GlobalSystemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Features_GlobalSystems_GlobalSystemId",
                table: "Features");

            migrationBuilder.DropColumn(
                name: "Body",
                table: "Features");

            migrationBuilder.DropColumn(
                name: "Comments",
                table: "Features");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Features");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Features");

            migrationBuilder.RenameColumn(
                name: "GlobalSystemId",
                table: "Features",
                newName: "Service_id");

            migrationBuilder.RenameIndex(
                name: "IX_Features_GlobalSystemId",
                table: "Features",
                newName: "IX_Features_Service_id");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Features",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Features_Services_Service_id",
                table: "Features",
                column: "Service_id",
                principalTable: "Services",
                principalColumn: "Service_id");
        }
    }
}
