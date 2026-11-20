using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addUniversalOrdersTblsSchemaup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Size_id",
                table: "UniversalOrderServices",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.CreateIndex(
                name: "IX_UniversalOrderServices_Size_id",
                table: "UniversalOrderServices",
                column: "Size_id");

            migrationBuilder.AddForeignKey(
                name: "FK_UniversalOrderServices_Sizes_Size_id",
                table: "UniversalOrderServices",
                column: "Size_id",
                principalTable: "Sizes",
                principalColumn: "Size_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UniversalOrderServices_Sizes_Size_id",
                table: "UniversalOrderServices");

            migrationBuilder.DropIndex(
                name: "IX_UniversalOrderServices_Size_id",
                table: "UniversalOrderServices");

            migrationBuilder.AlterColumn<decimal>(
                name: "Size_id",
                table: "UniversalOrderServices",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
