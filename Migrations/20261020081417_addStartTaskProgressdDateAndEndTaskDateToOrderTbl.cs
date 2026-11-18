using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addStartTaskProgressdDateAndEndTaskDateToOrderTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Sizes_Size_id",
                table: "OrderItems");

            migrationBuilder.AlterColumn<int>(
                name: "Size_id",
                table: "OrderItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateOnly>(
                name: "EndTaskDate",
                table: "GlobalOrders",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartTaskProgressdDate",
                table: "GlobalOrders",
                type: "date",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Sizes_Size_id",
                table: "OrderItems",
                column: "Size_id",
                principalTable: "Sizes",
                principalColumn: "Size_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Sizes_Size_id",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "EndTaskDate",
                table: "GlobalOrders");

            migrationBuilder.DropColumn(
                name: "StartTaskProgressdDate",
                table: "GlobalOrders");

            migrationBuilder.AlterColumn<int>(
                name: "Size_id",
                table: "OrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Sizes_Size_id",
                table: "OrderItems",
                column: "Size_id",
                principalTable: "Sizes",
                principalColumn: "Size_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
