using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addAssign_typeTblfj : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Features_FeatureId",
                table: "Comments");

            migrationBuilder.AddColumn<int>(
                name: "AssigneeId",
                table: "GlobalOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssigneeTypeId",
                table: "GlobalOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssignerId",
                table: "GlobalOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssignerTypeId",
                table: "GlobalOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FeatureId",
                table: "Comments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "GlobalOrderId",
                table: "Comments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Assign_types",
                columns: table => new
                {
                    Assign_typeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assign_types", x => x.Assign_typeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GlobalOrders_AssigneeTypeId",
                table: "GlobalOrders",
                column: "AssigneeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalOrders_AssignerTypeId",
                table: "GlobalOrders",
                column: "AssignerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_GlobalOrderId",
                table: "Comments",
                column: "GlobalOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Features_FeatureId",
                table: "Comments",
                column: "FeatureId",
                principalTable: "Features",
                principalColumn: "FeatureId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_GlobalOrders_GlobalOrderId",
                table: "Comments",
                column: "GlobalOrderId",
                principalTable: "GlobalOrders",
                principalColumn: "GlobalOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalOrders_Assign_types_AssigneeTypeId",
                table: "GlobalOrders",
                column: "AssigneeTypeId",
                principalTable: "Assign_types",
                principalColumn: "Assign_typeId");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalOrders_Assign_types_AssignerTypeId",
                table: "GlobalOrders",
                column: "AssignerTypeId",
                principalTable: "Assign_types",
                principalColumn: "Assign_typeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Features_FeatureId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_GlobalOrders_GlobalOrderId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_GlobalOrders_Assign_types_AssigneeTypeId",
                table: "GlobalOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_GlobalOrders_Assign_types_AssignerTypeId",
                table: "GlobalOrders");

            migrationBuilder.DropTable(
                name: "Assign_types");

            migrationBuilder.DropIndex(
                name: "IX_GlobalOrders_AssigneeTypeId",
                table: "GlobalOrders");

            migrationBuilder.DropIndex(
                name: "IX_GlobalOrders_AssignerTypeId",
                table: "GlobalOrders");

            migrationBuilder.DropIndex(
                name: "IX_Comments_GlobalOrderId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "AssigneeId",
                table: "GlobalOrders");

            migrationBuilder.DropColumn(
                name: "AssigneeTypeId",
                table: "GlobalOrders");

            migrationBuilder.DropColumn(
                name: "AssignerId",
                table: "GlobalOrders");

            migrationBuilder.DropColumn(
                name: "AssignerTypeId",
                table: "GlobalOrders");

            migrationBuilder.DropColumn(
                name: "GlobalOrderId",
                table: "Comments");

            migrationBuilder.AlterColumn<int>(
                name: "FeatureId",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Features_FeatureId",
                table: "Comments",
                column: "FeatureId",
                principalTable: "Features",
                principalColumn: "FeatureId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
