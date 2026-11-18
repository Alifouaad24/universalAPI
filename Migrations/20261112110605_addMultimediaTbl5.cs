using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addMultimediaTbl5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Multimedias_Businesses_Business_id",
                table: "Multimedias");

            migrationBuilder.AlterColumn<int>(
                name: "Business_id",
                table: "Multimedias",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Multimedias_Businesses_Business_id",
                table: "Multimedias",
                column: "Business_id",
                principalTable: "Businesses",
                principalColumn: "Business_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Multimedias_Businesses_Business_id",
                table: "Multimedias");

            migrationBuilder.AlterColumn<int>(
                name: "Business_id",
                table: "Multimedias",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Multimedias_Businesses_Business_id",
                table: "Multimedias",
                column: "Business_id",
                principalTable: "Businesses",
                principalColumn: "Business_id");
        }
    }
}
