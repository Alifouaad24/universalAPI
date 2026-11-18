using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class bindOrderWithImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_caseImages_Cases_CaseId",
                table: "caseImages");

            migrationBuilder.AlterColumn<int>(
                name: "CaseId",
                table: "caseImages",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "GlobalOrderId",
                table: "caseImages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_caseImages_GlobalOrderId",
                table: "caseImages",
                column: "GlobalOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_caseImages_Cases_CaseId",
                table: "caseImages",
                column: "CaseId",
                principalTable: "Cases",
                principalColumn: "CaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_caseImages_GlobalOrders_GlobalOrderId",
                table: "caseImages",
                column: "GlobalOrderId",
                principalTable: "GlobalOrders",
                principalColumn: "GlobalOrderId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_caseImages_Cases_CaseId",
                table: "caseImages");

            migrationBuilder.DropForeignKey(
                name: "FK_caseImages_GlobalOrders_GlobalOrderId",
                table: "caseImages");

            migrationBuilder.DropIndex(
                name: "IX_caseImages_GlobalOrderId",
                table: "caseImages");

            migrationBuilder.DropColumn(
                name: "GlobalOrderId",
                table: "caseImages");

            migrationBuilder.AlterColumn<int>(
                name: "CaseId",
                table: "caseImages",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_caseImages_Cases_CaseId",
                table: "caseImages",
                column: "CaseId",
                principalTable: "Cases",
                principalColumn: "CaseId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
