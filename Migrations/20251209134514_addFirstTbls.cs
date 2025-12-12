using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addFirstTbls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Address_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Line_1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Line_2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Post_code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Address_id);
                });

            migrationBuilder.CreateTable(
                name: "Business_types",
                columns: table => new
                {
                    Business_type_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Business_types", x => x.Business_type_id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    CountryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.CountryId);
                });

            migrationBuilder.CreateTable(
                name: "Businesses",
                columns: table => new
                {
                    Business_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Business_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    Is_active = table.Column<bool>(type: "bit", nullable: false),
                    Business_phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Business_webSite = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Business_fb = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Business_instgram = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Business_tiktok = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Business_google = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Business_youtube = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Business_whatsapp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Business_email = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Businesses", x => x.Business_id);
                    table.ForeignKey(
                        name: "FK_Businesses_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Activiities",
                columns: table => new
                {
                    Activity_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Business_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activiities", x => x.Activity_id);
                    table.ForeignKey(
                        name: "FK_Activiities_Businesses_Business_id",
                        column: x => x.Business_id,
                        principalTable: "Businesses",
                        principalColumn: "Business_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Business_Addresses",
                columns: table => new
                {
                    Business_id = table.Column<int>(type: "int", nullable: false),
                    Address_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Business_Addresses", x => new { x.Business_id, x.Address_id });
                    table.ForeignKey(
                        name: "FK_Business_Addresses_Addresses_Address_id",
                        column: x => x.Address_id,
                        principalTable: "Addresses",
                        principalColumn: "Address_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Business_Addresses_Businesses_Business_id",
                        column: x => x.Business_id,
                        principalTable: "Businesses",
                        principalColumn: "Business_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Business_BusinessTypes",
                columns: table => new
                {
                    Business_id = table.Column<int>(type: "int", nullable: false),
                    Business_type_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Business_BusinessTypes", x => new { x.Business_id, x.Business_type_id });
                    table.ForeignKey(
                        name: "FK_Business_BusinessTypes_Business_types_Business_type_id",
                        column: x => x.Business_type_id,
                        principalTable: "Business_types",
                        principalColumn: "Business_type_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Business_BusinessTypes_Businesses_Business_id",
                        column: x => x.Business_id,
                        principalTable: "Businesses",
                        principalColumn: "Business_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activiities_Business_id",
                table: "Activiities",
                column: "Business_id");

            migrationBuilder.CreateIndex(
                name: "IX_Business_Addresses_Address_id",
                table: "Business_Addresses",
                column: "Address_id");

            migrationBuilder.CreateIndex(
                name: "IX_Business_BusinessTypes_Business_type_id",
                table: "Business_BusinessTypes",
                column: "Business_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_Businesses_CountryId",
                table: "Businesses",
                column: "CountryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Activiities");

            migrationBuilder.DropTable(
                name: "Business_Addresses");

            migrationBuilder.DropTable(
                name: "Business_BusinessTypes");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Business_types");

            migrationBuilder.DropTable(
                name: "Businesses");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
