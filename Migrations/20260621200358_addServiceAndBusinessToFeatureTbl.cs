using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addServiceAndBusinessToFeatureTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Business_id",
                table: "Features",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Service_id",
                table: "Features",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Features_Business_id",
                table: "Features",
                column: "Business_id");

            migrationBuilder.CreateIndex(
                name: "IX_Features_Service_id",
                table: "Features",
                column: "Service_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Features_Businesses_Business_id",
                table: "Features",
                column: "Business_id",
                principalTable: "Businesses",
                principalColumn: "Business_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Features_Services_Service_id",
                table: "Features",
                column: "Service_id",
                principalTable: "Services",
                principalColumn: "Service_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Features_Businesses_Business_id",
                table: "Features");

            migrationBuilder.DropForeignKey(
                name: "FK_Features_Services_Service_id",
                table: "Features");

            migrationBuilder.DropIndex(
                name: "IX_Features_Business_id",
                table: "Features");

            migrationBuilder.DropIndex(
                name: "IX_Features_Service_id",
                table: "Features");

            migrationBuilder.DropColumn(
                name: "Business_id",
                table: "Features");

            migrationBuilder.DropColumn(
                name: "Service_id",
                table: "Features");
        }
    }
}
