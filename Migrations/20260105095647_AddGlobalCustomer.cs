using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class AddGlobalCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GlobalCustomers",
                columns: table => new
                {
                    GlobalCustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerMobile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country_id = table.Column<int>(type: "int", nullable: true),
                    AddressId = table.Column<int>(type: "int", nullable: true),
                    Insert_on = table.Column<DateOnly>(type: "date", nullable: false),
                    Insert_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    visible = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalCustomers", x => x.GlobalCustomerId);
                    table.ForeignKey(
                        name: "FK_GlobalCustomers_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Address_id");
                    table.ForeignKey(
                        name: "FK_GlobalCustomers_Countries_Country_id",
                        column: x => x.Country_id,
                        principalTable: "Countries",
                        principalColumn: "CountryId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_GlobalCustomers_AddressId",
                table: "GlobalCustomers",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalCustomers_Country_id",
                table: "GlobalCustomers",
                column: "Country_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GlobalCustomers");
        }
    }
}
