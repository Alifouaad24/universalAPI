using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addBusinessPlatformsTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Platforms_Businesses_BusinessId",
                table: "Platforms");

            migrationBuilder.RenameColumn(
                name: "BusinessId",
                table: "Platforms",
                newName: "Business_id");

            migrationBuilder.RenameIndex(
                name: "IX_Platforms_BusinessId",
                table: "Platforms",
                newName: "IX_Platforms_Business_id");

            migrationBuilder.CreateTable(
                name: "BusinessPlatforms",
                columns: table => new
                {
                    BusinessPlatformsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Business_id = table.Column<int>(type: "int", nullable: true),
                    Platform_id = table.Column<int>(type: "int", nullable: true),
                    ForScraping = table.Column<bool>(type: "bit", nullable: true),
                    ForSelling = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessPlatforms", x => x.BusinessPlatformsId);
                    table.ForeignKey(
                        name: "FK_BusinessPlatforms_Businesses_Business_id",
                        column: x => x.Business_id,
                        principalTable: "Businesses",
                        principalColumn: "Business_id");
                    table.ForeignKey(
                        name: "FK_BusinessPlatforms_Platforms_Platform_id",
                        column: x => x.Platform_id,
                        principalTable: "Platforms",
                        principalColumn: "Platform_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessPlatforms_Business_id",
                table: "BusinessPlatforms",
                column: "Business_id");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessPlatforms_Platform_id",
                table: "BusinessPlatforms",
                column: "Platform_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Platforms_Businesses_Business_id",
                table: "Platforms",
                column: "Business_id",
                principalTable: "Businesses",
                principalColumn: "Business_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Platforms_Businesses_Business_id",
                table: "Platforms");

            migrationBuilder.DropTable(
                name: "BusinessPlatforms");

            migrationBuilder.RenameColumn(
                name: "Business_id",
                table: "Platforms",
                newName: "BusinessId");

            migrationBuilder.RenameIndex(
                name: "IX_Platforms_Business_id",
                table: "Platforms",
                newName: "IX_Platforms_BusinessId");

            migrationBuilder.AddForeignKey(
                name: "FK_Platforms_Businesses_BusinessId",
                table: "Platforms",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Business_id");
        }
    }
}
