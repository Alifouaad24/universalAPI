using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addTaarefTblV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Taarefs_Businesses_Business_id",
                table: "Taarefs");

            migrationBuilder.DropForeignKey(
                name: "FK_Taarefs_Currencies_CurrencyId",
                table: "Taarefs");

            migrationBuilder.AlterColumn<int>(
                name: "CurrencyId",
                table: "Taarefs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Business_id",
                table: "Taarefs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Taarefs_Businesses_Business_id",
                table: "Taarefs",
                column: "Business_id",
                principalTable: "Businesses",
                principalColumn: "Business_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Taarefs_Currencies_CurrencyId",
                table: "Taarefs",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "CurrencyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Taarefs_Businesses_Business_id",
                table: "Taarefs");

            migrationBuilder.DropForeignKey(
                name: "FK_Taarefs_Currencies_CurrencyId",
                table: "Taarefs");

            migrationBuilder.AlterColumn<int>(
                name: "CurrencyId",
                table: "Taarefs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Business_id",
                table: "Taarefs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Taarefs_Businesses_Business_id",
                table: "Taarefs",
                column: "Business_id",
                principalTable: "Businesses",
                principalColumn: "Business_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Taarefs_Currencies_CurrencyId",
                table: "Taarefs",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "CurrencyId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
