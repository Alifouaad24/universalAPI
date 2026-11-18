using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class setBusiness_idInActivityNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activiities_Businesses_Business_id",
                table: "Activiities");

            migrationBuilder.AlterColumn<int>(
                name: "Business_id",
                table: "Activiities",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Activiities_Businesses_Business_id",
                table: "Activiities",
                column: "Business_id",
                principalTable: "Businesses",
                principalColumn: "Business_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activiities_Businesses_Business_id",
                table: "Activiities");

            migrationBuilder.AlterColumn<int>(
                name: "Business_id",
                table: "Activiities",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Activiities_Businesses_Business_id",
                table: "Activiities",
                column: "Business_id",
                principalTable: "Businesses",
                principalColumn: "Business_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
