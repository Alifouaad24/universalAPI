using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addbusinessIdToBrandTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_GlobalCustomers_GlobalCustomerId",
                table: "Items");

            migrationBuilder.AlterColumn<int>(
                name: "GlobalCustomerId",
                table: "Items",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "BusinessId",
                table: "CarBrands",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_GlobalCustomers_GlobalCustomerId",
                table: "Items",
                column: "GlobalCustomerId",
                principalTable: "GlobalCustomers",
                principalColumn: "GlobalCustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_GlobalCustomers_GlobalCustomerId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "BusinessId",
                table: "CarBrands");

            migrationBuilder.AlterColumn<int>(
                name: "GlobalCustomerId",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_GlobalCustomers_GlobalCustomerId",
                table: "Items",
                column: "GlobalCustomerId",
                principalTable: "GlobalCustomers",
                principalColumn: "GlobalCustomerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
