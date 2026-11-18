using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addItemConitioTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ItemConditionId",
                table: "Inventories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EbayCategoryId",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ItemConditions",
                columns: table => new
                {
                    ItemConditionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EbayConditionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemConditions", x => x.ItemConditionId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_ItemConditionId",
                table: "Inventories",
                column: "ItemConditionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventories_ItemConditions_ItemConditionId",
                table: "Inventories",
                column: "ItemConditionId",
                principalTable: "ItemConditions",
                principalColumn: "ItemConditionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventories_ItemConditions_ItemConditionId",
                table: "Inventories");

            migrationBuilder.DropTable(
                name: "ItemConditions");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_ItemConditionId",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "ItemConditionId",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "EbayCategoryId",
                table: "Categories");
        }
    }
}
