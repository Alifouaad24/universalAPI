using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addAssetTblWithBus_asset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    AssetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetName = table.Column<int>(type: "int", nullable: false),
                    AssetType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.AssetId);
                });

            migrationBuilder.CreateTable(
                name: "Business_Assets",
                columns: table => new
                {
                    Business_AssetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetId = table.Column<int>(type: "int", nullable: true),
                    Business_id = table.Column<int>(type: "int", nullable: true),
                    Account_Id = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Business_Assets", x => x.Business_AssetId);
                    table.ForeignKey(
                        name: "FK_Business_Assets_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "AssetId");
                    table.ForeignKey(
                        name: "FK_Business_Assets_Businesses_Business_id",
                        column: x => x.Business_id,
                        principalTable: "Businesses",
                        principalColumn: "Business_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Business_Assets_AssetId",
                table: "Business_Assets",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Business_Assets_Business_id",
                table: "Business_Assets",
                column: "Business_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Business_Assets");

            migrationBuilder.DropTable(
                name: "Assets");
        }
    }
}
