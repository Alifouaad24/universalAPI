using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addServiceTBL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Service_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activity_id = table.Column<int>(type: "int", nullable: true),
                    Insert_on = table.Column<DateOnly>(type: "date", nullable: false),
                    Insert_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    visible = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Service_id);
                    table.ForeignKey(
                        name: "FK_Services_Activiities_Activity_id",
                        column: x => x.Activity_id,
                        principalTable: "Activiities",
                        principalColumn: "Activity_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Services_Activity_id",
                table: "Services",
                column: "Activity_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Services");
        }
    }
}
