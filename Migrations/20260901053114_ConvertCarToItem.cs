using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class ConvertCarToItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GlobalOrders_CarInfoTbls_CarInfoTblId",
                table: "GlobalOrders");

            migrationBuilder.RenameColumn(
                name: "CarInfoTblId",
                table: "GlobalOrders",
                newName: "ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_GlobalOrders_CarInfoTblId",
                table: "GlobalOrders",
                newName: "IX_GlobalOrders_ItemId");

            migrationBuilder.AddColumn<int>(
                name: "CarModelId",
                table: "Items",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CarYearId",
                table: "Items",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GlobalCustomerId",
                table: "Items",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "VinNumber",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_CarModelId",
                table: "Items",
                column: "CarModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_CarYearId",
                table: "Items",
                column: "CarYearId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_GlobalCustomerId",
                table: "Items",
                column: "GlobalCustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalOrders_Items_ItemId",
                table: "GlobalOrders",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_CarModels_CarModelId",
                table: "Items",
                column: "CarModelId",
                principalTable: "CarModels",
                principalColumn: "CarModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_CarYears_CarYearId",
                table: "Items",
                column: "CarYearId",
                principalTable: "CarYears",
                principalColumn: "CarYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_GlobalCustomers_GlobalCustomerId",
                table: "Items",
                column: "GlobalCustomerId",
                principalTable: "GlobalCustomers",
                principalColumn: "GlobalCustomerId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GlobalOrders_Items_ItemId",
                table: "GlobalOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_CarModels_CarModelId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_CarYears_CarYearId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_GlobalCustomers_GlobalCustomerId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_CarModelId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_CarYearId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_GlobalCustomerId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "CarModelId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "CarYearId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "GlobalCustomerId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "VinNumber",
                table: "Items");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "GlobalOrders",
                newName: "CarInfoTblId");

            migrationBuilder.RenameIndex(
                name: "IX_GlobalOrders_ItemId",
                table: "GlobalOrders",
                newName: "IX_GlobalOrders_CarInfoTblId");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalOrders_CarInfoTbls_CarInfoTblId",
                table: "GlobalOrders",
                column: "CarInfoTblId",
                principalTable: "CarInfoTbls",
                principalColumn: "CarInfoTblId");
        }
    }
}
