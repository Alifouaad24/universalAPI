using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addBusActTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Business_Activitiy_Activiities_Activity_id",
                table: "Business_Activitiy");

            migrationBuilder.DropForeignKey(
                name: "FK_Business_Activitiy_Businesses_Business_id",
                table: "Business_Activitiy");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Business_Activitiy",
                table: "Business_Activitiy");

            migrationBuilder.RenameTable(
                name: "Business_Activitiy",
                newName: "Business_Activitiies");

            migrationBuilder.RenameIndex(
                name: "IX_Business_Activitiy_Business_id",
                table: "Business_Activitiies",
                newName: "IX_Business_Activitiies_Business_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Business_Activitiies",
                table: "Business_Activitiies",
                columns: new[] { "Activity_id", "Business_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_Business_Activitiies_Activiities_Activity_id",
                table: "Business_Activitiies",
                column: "Activity_id",
                principalTable: "Activiities",
                principalColumn: "Activity_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Business_Activitiies_Businesses_Business_id",
                table: "Business_Activitiies",
                column: "Business_id",
                principalTable: "Businesses",
                principalColumn: "Business_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Business_Activitiies_Activiities_Activity_id",
                table: "Business_Activitiies");

            migrationBuilder.DropForeignKey(
                name: "FK_Business_Activitiies_Businesses_Business_id",
                table: "Business_Activitiies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Business_Activitiies",
                table: "Business_Activitiies");

            migrationBuilder.RenameTable(
                name: "Business_Activitiies",
                newName: "Business_Activitiy");

            migrationBuilder.RenameIndex(
                name: "IX_Business_Activitiies_Business_id",
                table: "Business_Activitiy",
                newName: "IX_Business_Activitiy_Business_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Business_Activitiy",
                table: "Business_Activitiy",
                columns: new[] { "Activity_id", "Business_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_Business_Activitiy_Activiities_Activity_id",
                table: "Business_Activitiy",
                column: "Activity_id",
                principalTable: "Activiities",
                principalColumn: "Activity_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Business_Activitiy_Businesses_Business_id",
                table: "Business_Activitiy",
                column: "Business_id",
                principalTable: "Businesses",
                principalColumn: "Business_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
