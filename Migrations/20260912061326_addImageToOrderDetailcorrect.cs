using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addImageToOrderDetailcorrect : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_caseImages_GlobalOrders_GlobalOrderId",
                table: "caseImages");

            migrationBuilder.DropForeignKey(
                name: "FK_GlobalOrderDetails_caseImages_caseImageId",
                table: "GlobalOrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_GlobalOrderDetails_caseImageId",
                table: "GlobalOrderDetails");

            migrationBuilder.DropColumn(
                name: "caseImageId",
                table: "GlobalOrderDetails");

            migrationBuilder.DropColumn(
                name: "GlobalOrderDetailId",
                table: "caseImages");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "GlobalOrderDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GlobalOrderId",
                table: "caseImages",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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
                name: "FK_caseImages_GlobalOrders_GlobalOrderId",
                table: "caseImages");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "GlobalOrderDetails");

            migrationBuilder.AddColumn<int>(
                name: "caseImageId",
                table: "GlobalOrderDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GlobalOrderId",
                table: "caseImages",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "GlobalOrderDetailId",
                table: "caseImages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GlobalOrderDetails_caseImageId",
                table: "GlobalOrderDetails",
                column: "caseImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_caseImages_GlobalOrders_GlobalOrderId",
                table: "caseImages",
                column: "GlobalOrderId",
                principalTable: "GlobalOrders",
                principalColumn: "GlobalOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalOrderDetails_caseImages_caseImageId",
                table: "GlobalOrderDetails",
                column: "caseImageId",
                principalTable: "caseImages",
                principalColumn: "caseImageId");
        }
    }
}
