using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addBusinessGroupBusinessesTbls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusinessGroups",
                columns: table => new
                {
                    BusinessGroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessGroups", x => x.BusinessGroupId);
                });

            migrationBuilder.CreateTable(
                name: "BusinessGroupBusinesses",
                columns: table => new
                {
                    BusinessGroupBusinessesId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Business_id = table.Column<int>(type: "int", nullable: false),
                    BusinessGroupsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessGroupBusinesses", x => x.BusinessGroupBusinessesId);
                    table.ForeignKey(
                        name: "FK_BusinessGroupBusinesses_BusinessGroups_BusinessGroupsId",
                        column: x => x.BusinessGroupsId,
                        principalTable: "BusinessGroups",
                        principalColumn: "BusinessGroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BusinessGroupBusinesses_Businesses_Business_id",
                        column: x => x.Business_id,
                        principalTable: "Businesses",
                        principalColumn: "Business_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessGroupBusinesses_Business_id",
                table: "BusinessGroupBusinesses",
                column: "Business_id");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessGroupBusinesses_BusinessGroupsId",
                table: "BusinessGroupBusinesses",
                column: "BusinessGroupsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessGroupBusinesses");

            migrationBuilder.DropTable(
                name: "BusinessGroups");
        }
    }
}
