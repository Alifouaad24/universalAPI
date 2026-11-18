using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addBusinessToShippingList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Business_id",
                table: "ShippingLists",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingLists_Business_id",
                table: "ShippingLists",
                column: "Business_id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingLists_Businesses_Business_id",
                table: "ShippingLists",
                column: "Business_id",
                principalTable: "Businesses",
                principalColumn: "Business_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingLists_Businesses_Business_id",
                table: "ShippingLists");

            migrationBuilder.DropIndex(
                name: "IX_ShippingLists_Business_id",
                table: "ShippingLists");

            migrationBuilder.DropColumn(
                name: "Business_id",
                table: "ShippingLists");
        }
    }
}
