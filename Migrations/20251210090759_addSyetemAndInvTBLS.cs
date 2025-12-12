using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addSyetemAndInvTBLS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Insert_by",
                table: "Countries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "Insert_on",
                table: "Countries",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<bool>(
                name: "visible",
                table: "Countries",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Insert_by",
                table: "Businesses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "Insert_on",
                table: "Businesses",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<bool>(
                name: "visible",
                table: "Businesses",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Insert_by",
                table: "Business_types",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "Insert_on",
                table: "Business_types",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<bool>(
                name: "visible",
                table: "Business_types",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Insert_by",
                table: "Business_BusinessTypes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "Insert_on",
                table: "Business_BusinessTypes",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<bool>(
                name: "visible",
                table: "Business_BusinessTypes",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Insert_by",
                table: "Business_Addresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "Insert_on",
                table: "Business_Addresses",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<bool>(
                name: "visible",
                table: "Business_Addresses",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Insert_by",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "Insert_on",
                table: "Addresses",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<bool>(
                name: "visible",
                table: "Addresses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Insert_by",
                table: "Activiities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "Insert_on",
                table: "Activiities",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<bool>(
                name: "visible",
                table: "Activiities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Category_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Insert_on = table.Column<DateOnly>(type: "date", nullable: false),
                    Insert_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    visible = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Category_id);
                });

            migrationBuilder.CreateTable(
                name: "Platforms",
                columns: table => new
                {
                    Platform_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Insert_on = table.Column<DateOnly>(type: "date", nullable: false),
                    Insert_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    visible = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Platforms", x => x.Platform_id);
                });

            migrationBuilder.CreateTable(
                name: "System_gatewaies",
                columns: table => new
                {
                    System_gateway_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Insert_on = table.Column<DateOnly>(type: "date", nullable: false),
                    Insert_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    visible = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_System_gatewaies", x => x.System_gateway_id);
                });

            migrationBuilder.CreateTable(
                name: "System_sectores",
                columns: table => new
                {
                    System_sectore_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Insert_on = table.Column<DateOnly>(type: "date", nullable: false),
                    Insert_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    visible = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_System_sectores", x => x.System_sectore_id);
                });

            migrationBuilder.CreateTable(
                name: "Sizes",
                columns: table => new
                {
                    Size_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category_id = table.Column<int>(type: "int", nullable: true),
                    Insert_on = table.Column<DateOnly>(type: "date", nullable: false),
                    Insert_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    visible = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sizes", x => x.Size_id);
                    table.ForeignKey(
                        name: "FK_Sizes_Categories_Category_id",
                        column: x => x.Category_id,
                        principalTable: "Categories",
                        principalColumn: "Category_id");
                });

            migrationBuilder.CreateTable(
                name: "System_sectore_details",
                columns: table => new
                {
                    System_sectore_details_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    System_sectore_id = table.Column<int>(type: "int", nullable: true),
                    Insert_on = table.Column<DateOnly>(type: "date", nullable: false),
                    Insert_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    visible = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_System_sectore_details", x => x.System_sectore_details_id);
                    table.ForeignKey(
                        name: "FK_System_sectore_details_System_sectores_System_sectore_id",
                        column: x => x.System_sectore_id,
                        principalTable: "System_sectores",
                        principalColumn: "System_sectore_id");
                });

            migrationBuilder.CreateTable(
                name: "Inventories",
                columns: table => new
                {
                    Inventory_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Product_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Height = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Depth = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Upc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sku = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InternetId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Product_description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Platform_id = table.Column<int>(type: "int", nullable: true),
                    Size_id1 = table.Column<int>(type: "int", nullable: true),
                    Size_id = table.Column<int>(type: "int", nullable: true),
                    Category_id = table.Column<int>(type: "int", nullable: true),
                    Insert_on = table.Column<DateOnly>(type: "date", nullable: false),
                    Insert_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    visible = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventories", x => x.Inventory_id);
                    table.ForeignKey(
                        name: "FK_Inventories_Categories_Category_id",
                        column: x => x.Category_id,
                        principalTable: "Categories",
                        principalColumn: "Category_id");
                    table.ForeignKey(
                        name: "FK_Inventories_Platforms_Platform_id",
                        column: x => x.Platform_id,
                        principalTable: "Platforms",
                        principalColumn: "Platform_id");
                    table.ForeignKey(
                        name: "FK_Inventories_Sizes_Size_id1",
                        column: x => x.Size_id1,
                        principalTable: "Sizes",
                        principalColumn: "Size_id");
                });

            migrationBuilder.CreateTable(
                name: "Inventory_businesses",
                columns: table => new
                {
                    Inventory_business_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    Inventory_id = table.Column<int>(type: "int", nullable: false),
                    Business_id = table.Column<int>(type: "int", nullable: false),
                    Insert_on = table.Column<DateOnly>(type: "date", nullable: false),
                    Insert_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    visible = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory_businesses", x => x.Inventory_business_id);
                    table.ForeignKey(
                        name: "FK_Inventory_businesses_Businesses_Business_id",
                        column: x => x.Business_id,
                        principalTable: "Businesses",
                        principalColumn: "Business_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Inventory_businesses_Inventories_Inventory_id",
                        column: x => x.Inventory_id,
                        principalTable: "Inventories",
                        principalColumn: "Inventory_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Inventoy_images",
                columns: table => new
                {
                    Inventoy_images_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Inventoy_images_url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Inventory_id = table.Column<int>(type: "int", nullable: false),
                    Insert_on = table.Column<DateOnly>(type: "date", nullable: false),
                    Insert_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    visible = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventoy_images", x => x.Inventoy_images_id);
                    table.ForeignKey(
                        name: "FK_Inventoy_images_Inventories_Inventory_id",
                        column: x => x.Inventory_id,
                        principalTable: "Inventories",
                        principalColumn: "Inventory_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_Category_id",
                table: "Inventories",
                column: "Category_id");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_Platform_id",
                table: "Inventories",
                column: "Platform_id");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_Size_id1",
                table: "Inventories",
                column: "Size_id1");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_businesses_Business_id",
                table: "Inventory_businesses",
                column: "Business_id");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_businesses_Inventory_id",
                table: "Inventory_businesses",
                column: "Inventory_id");

            migrationBuilder.CreateIndex(
                name: "IX_Inventoy_images_Inventory_id",
                table: "Inventoy_images",
                column: "Inventory_id");

            migrationBuilder.CreateIndex(
                name: "IX_Sizes_Category_id",
                table: "Sizes",
                column: "Category_id");

            migrationBuilder.CreateIndex(
                name: "IX_System_sectore_details_System_sectore_id",
                table: "System_sectore_details",
                column: "System_sectore_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inventory_businesses");

            migrationBuilder.DropTable(
                name: "Inventoy_images");

            migrationBuilder.DropTable(
                name: "System_gatewaies");

            migrationBuilder.DropTable(
                name: "System_sectore_details");

            migrationBuilder.DropTable(
                name: "Inventories");

            migrationBuilder.DropTable(
                name: "System_sectores");

            migrationBuilder.DropTable(
                name: "Platforms");

            migrationBuilder.DropTable(
                name: "Sizes");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropColumn(
                name: "Insert_by",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "Insert_on",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "visible",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "Insert_by",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "Insert_on",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "visible",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "Insert_by",
                table: "Business_types");

            migrationBuilder.DropColumn(
                name: "Insert_on",
                table: "Business_types");

            migrationBuilder.DropColumn(
                name: "visible",
                table: "Business_types");

            migrationBuilder.DropColumn(
                name: "Insert_by",
                table: "Business_BusinessTypes");

            migrationBuilder.DropColumn(
                name: "Insert_on",
                table: "Business_BusinessTypes");

            migrationBuilder.DropColumn(
                name: "visible",
                table: "Business_BusinessTypes");

            migrationBuilder.DropColumn(
                name: "Insert_by",
                table: "Business_Addresses");

            migrationBuilder.DropColumn(
                name: "Insert_on",
                table: "Business_Addresses");

            migrationBuilder.DropColumn(
                name: "visible",
                table: "Business_Addresses");

            migrationBuilder.DropColumn(
                name: "Insert_by",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "Insert_on",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "visible",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "Insert_by",
                table: "Activiities");

            migrationBuilder.DropColumn(
                name: "Insert_on",
                table: "Activiities");

            migrationBuilder.DropColumn(
                name: "visible",
                table: "Activiities");
        }
    }
}
