using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class reBindBusinessWithActivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activiities_Businesses_Business_id",
                table: "Activiities");

            migrationBuilder.DropIndex(
                name: "IX_Activiities_Business_id",
                table: "Activiities");

            migrationBuilder.DropColumn(
                name: "Brand",
                table: "CarInfoTbls");

            migrationBuilder.DropColumn(
                name: "Model",
                table: "CarInfoTbls");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "CarInfoTbls");

            migrationBuilder.DropColumn(
                name: "Business_id",
                table: "Activiities");

            migrationBuilder.AlterColumn<string>(
                name: "ServingName",
                table: "Servings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Services",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerMobile",
                table: "GlobalCustomers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "CarModelId",
                table: "CarInfoTbls",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CarBrandName",
                table: "CarBrands",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Business_name",
                table: "Businesses",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Business_types",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Line_1",
                table: "Addresses",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Activiities",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Business_Activitiy",
                columns: table => new
                {
                    Business_id = table.Column<int>(type: "int", nullable: false),
                    Activity_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Business_Activitiy", x => new { x.Activity_id, x.Business_id });
                    table.ForeignKey(
                        name: "FK_Business_Activitiy_Activiities_Activity_id",
                        column: x => x.Activity_id,
                        principalTable: "Activiities",
                        principalColumn: "Activity_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Business_Activitiy_Businesses_Business_id",
                        column: x => x.Business_id,
                        principalTable: "Businesses",
                        principalColumn: "Business_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarModels",
                columns: table => new
                {
                    CarModelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarModelName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CarBrandId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarModels", x => x.CarModelId);
                    table.ForeignKey(
                        name: "FK_CarModels_CarBrands_CarBrandId",
                        column: x => x.CarBrandId,
                        principalTable: "CarBrands",
                        principalColumn: "CarBrandId");
                });

            migrationBuilder.CreateTable(
                name: "CarYears",
                columns: table => new
                {
                    CarYearId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarYearNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CarModelId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarYears", x => x.CarYearId);
                    table.ForeignKey(
                        name: "FK_CarYears_CarModels_CarModelId",
                        column: x => x.CarModelId,
                        principalTable: "CarModels",
                        principalColumn: "CarModelId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Services_Description",
                table: "Services",
                column: "Description");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalCustomers_CustomerMobile",
                table: "GlobalCustomers",
                column: "CustomerMobile");

            migrationBuilder.CreateIndex(
                name: "IX_CarInfoTbls_CarModelId",
                table: "CarInfoTbls",
                column: "CarModelId");

            migrationBuilder.CreateIndex(
                name: "IX_CarBrands_CarBrandName",
                table: "CarBrands",
                column: "CarBrandName");

            migrationBuilder.CreateIndex(
                name: "IX_Businesses_Business_name",
                table: "Businesses",
                column: "Business_name");

            migrationBuilder.CreateIndex(
                name: "IX_Business_types_Description",
                table: "Business_types",
                column: "Description");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_Line_1",
                table: "Addresses",
                column: "Line_1");

            migrationBuilder.CreateIndex(
                name: "IX_Activiities_Description",
                table: "Activiities",
                column: "Description");

            migrationBuilder.CreateIndex(
                name: "IX_Business_Activitiy_Business_id",
                table: "Business_Activitiy",
                column: "Business_id");

            migrationBuilder.CreateIndex(
                name: "IX_CarModels_CarBrandId",
                table: "CarModels",
                column: "CarBrandId");

            migrationBuilder.CreateIndex(
                name: "IX_CarModels_CarModelName",
                table: "CarModels",
                column: "CarModelName");

            migrationBuilder.CreateIndex(
                name: "IX_CarYears_CarModelId",
                table: "CarYears",
                column: "CarModelId");

            migrationBuilder.CreateIndex(
                name: "IX_CarYears_CarYearNumber",
                table: "CarYears",
                column: "CarYearNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_CarInfoTbls_CarModels_CarModelId",
                table: "CarInfoTbls",
                column: "CarModelId",
                principalTable: "CarModels",
                principalColumn: "CarModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarInfoTbls_CarModels_CarModelId",
                table: "CarInfoTbls");

            migrationBuilder.DropTable(
                name: "Business_Activitiy");

            migrationBuilder.DropTable(
                name: "CarYears");

            migrationBuilder.DropTable(
                name: "CarModels");

            migrationBuilder.DropIndex(
                name: "IX_Services_Description",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_GlobalCustomers_CustomerMobile",
                table: "GlobalCustomers");

            migrationBuilder.DropIndex(
                name: "IX_CarInfoTbls_CarModelId",
                table: "CarInfoTbls");

            migrationBuilder.DropIndex(
                name: "IX_CarBrands_CarBrandName",
                table: "CarBrands");

            migrationBuilder.DropIndex(
                name: "IX_Businesses_Business_name",
                table: "Businesses");

            migrationBuilder.DropIndex(
                name: "IX_Business_types_Description",
                table: "Business_types");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_Line_1",
                table: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_Activiities_Description",
                table: "Activiities");

            migrationBuilder.DropColumn(
                name: "CarModelId",
                table: "CarInfoTbls");

            migrationBuilder.AlterColumn<string>(
                name: "ServingName",
                table: "Servings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Services",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerMobile",
                table: "GlobalCustomers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "Brand",
                table: "CarInfoTbls",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Model",
                table: "CarInfoTbls",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Year",
                table: "CarInfoTbls",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CarBrandName",
                table: "CarBrands",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Business_name",
                table: "Businesses",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Business_types",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Line_1",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Activiities",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Business_id",
                table: "Activiities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Activiities_Business_id",
                table: "Activiities",
                column: "Business_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Activiities_Businesses_Business_id",
                table: "Activiities",
                column: "Business_id",
                principalTable: "Businesses",
                principalColumn: "Business_id");
        }
    }
}
