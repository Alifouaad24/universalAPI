using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addComplimentLogicTbls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComplaintTypes",
                columns: table => new
                {
                    ComplaintTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplaintTypes", x => x.ComplaintTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Complaint_Values",
                columns: table => new
                {
                    Complaint_ValueId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComplaintTypeId = table.Column<int>(type: "int", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Complaint_Values", x => x.Complaint_ValueId);
                    table.ForeignKey(
                        name: "FK_Complaint_Values_ComplaintTypes_ComplaintTypeId",
                        column: x => x.ComplaintTypeId,
                        principalTable: "ComplaintTypes",
                        principalColumn: "ComplaintTypeId");
                });

            migrationBuilder.CreateTable(
                name: "Complaints",
                columns: table => new
                {
                    ComplaintId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Complaint_ValueId = table.Column<int>(type: "int", nullable: true),
                    GlobalOrderId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Complaints", x => x.ComplaintId);
                    table.ForeignKey(
                        name: "FK_Complaints_Complaint_Values_Complaint_ValueId",
                        column: x => x.Complaint_ValueId,
                        principalTable: "Complaint_Values",
                        principalColumn: "Complaint_ValueId");
                    table.ForeignKey(
                        name: "FK_Complaints_GlobalOrders_GlobalOrderId",
                        column: x => x.GlobalOrderId,
                        principalTable: "GlobalOrders",
                        principalColumn: "GlobalOrderId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Complaint_Values_ComplaintTypeId",
                table: "Complaint_Values",
                column: "ComplaintTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_Complaint_ValueId",
                table: "Complaints",
                column: "Complaint_ValueId");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_GlobalOrderId",
                table: "Complaints",
                column: "GlobalOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Complaints");

            migrationBuilder.DropTable(
                name: "Complaint_Values");

            migrationBuilder.DropTable(
                name: "ComplaintTypes");
        }
    }
}
