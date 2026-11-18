using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addBusiness_idToCustomerTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Business_id",
                table: "GlobalCustomers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GlobalCustomers_Business_id",
                table: "GlobalCustomers",
                column: "Business_id");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalCustomers_Businesses_Business_id",
                table: "GlobalCustomers",
                column: "Business_id",
                principalTable: "Businesses",
                principalColumn: "Business_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GlobalCustomers_Businesses_Business_id",
                table: "GlobalCustomers");

            migrationBuilder.DropIndex(
                name: "IX_GlobalCustomers_Business_id",
                table: "GlobalCustomers");

            migrationBuilder.DropColumn(
                name: "Business_id",
                table: "GlobalCustomers");
        }
    }
}
