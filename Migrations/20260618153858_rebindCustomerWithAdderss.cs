using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class rebindCustomerWithAdderss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GlobalCustomers_Addresses_AddressId",
                table: "GlobalCustomers");

            migrationBuilder.DropIndex(
                name: "IX_GlobalCustomers_AddressId",
                table: "GlobalCustomers");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "GlobalCustomers");

            migrationBuilder.AddColumn<int>(
                name: "GlobalCustomerId",
                table: "Addresses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_GlobalCustomerId",
                table: "Addresses",
                column: "GlobalCustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_GlobalCustomers_GlobalCustomerId",
                table: "Addresses",
                column: "GlobalCustomerId",
                principalTable: "GlobalCustomers",
                principalColumn: "GlobalCustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_GlobalCustomers_GlobalCustomerId",
                table: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_GlobalCustomerId",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "GlobalCustomerId",
                table: "Addresses");

            migrationBuilder.AddColumn<int>(
                name: "AddressId",
                table: "GlobalCustomers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GlobalCustomers_AddressId",
                table: "GlobalCustomers",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalCustomers_Addresses_AddressId",
                table: "GlobalCustomers",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Address_id");
        }
    }
}
