using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addPricingCatsTblLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NackName",
                table: "ShippingLists",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PricingCarts",
                columns: table => new
                {
                    PricingCartId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricingCarts", x => x.PricingCartId);
                });

            migrationBuilder.CreateTable(
                name: "PricingCartItems",
                columns: table => new
                {
                    PricingCartItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PricingCartId = table.Column<int>(type: "int", nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricingCartItems", x => x.PricingCartItemId);
                    table.ForeignKey(
                        name: "FK_PricingCartItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId");
                    table.ForeignKey(
                        name: "FK_PricingCartItems_PricingCarts_PricingCartId",
                        column: x => x.PricingCartId,
                        principalTable: "PricingCarts",
                        principalColumn: "PricingCartId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PricingCartItems_ItemId",
                table: "PricingCartItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PricingCartItems_PricingCartId",
                table: "PricingCartItems",
                column: "PricingCartId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PricingCartItems");

            migrationBuilder.DropTable(
                name: "PricingCarts");

            migrationBuilder.DropColumn(
                name: "NackName",
                table: "ShippingLists");
        }
    }
}
