using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addShippingTbls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Logistics",
                columns: table => new
                {
                    LogisticId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Business_id = table.Column<int>(type: "int", nullable: true),
                    CurrencyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logistics", x => x.LogisticId);
                    table.ForeignKey(
                        name: "FK_Logistics_Businesses_Business_id",
                        column: x => x.Business_id,
                        principalTable: "Businesses",
                        principalColumn: "Business_id");
                    table.ForeignKey(
                        name: "FK_Logistics_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId");
                });

            migrationBuilder.CreateTable(
                name: "ShippingTypes",
                columns: table => new
                {
                    ShippingTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingTypes", x => x.ShippingTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Weight_Units",
                columns: table => new
                {
                    Weight_UnitId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weight_Units", x => x.Weight_UnitId);
                });

            migrationBuilder.CreateTable(
                name: "Logistic_shippingType",
                columns: table => new
                {
                    Logistic_shippingTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LogisticId = table.Column<int>(type: "int", nullable: true),
                    ShippingTypeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logistic_shippingType", x => x.Logistic_shippingTypeId);
                    table.ForeignKey(
                        name: "FK_Logistic_shippingType_Logistics_LogisticId",
                        column: x => x.LogisticId,
                        principalTable: "Logistics",
                        principalColumn: "LogisticId");
                    table.ForeignKey(
                        name: "FK_Logistic_shippingType_ShippingTypes_ShippingTypeId",
                        column: x => x.ShippingTypeId,
                        principalTable: "ShippingTypes",
                        principalColumn: "ShippingTypeId");
                });

            migrationBuilder.CreateTable(
                name: "ShippingCosts",
                columns: table => new
                {
                    ShippingCostId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Business_id = table.Column<int>(type: "int", nullable: true),
                    Weight_UnitId = table.Column<int>(type: "int", nullable: true),
                    Weight_Value = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingCosts", x => x.ShippingCostId);
                    table.ForeignKey(
                        name: "FK_ShippingCosts_Businesses_Business_id",
                        column: x => x.Business_id,
                        principalTable: "Businesses",
                        principalColumn: "Business_id");
                    table.ForeignKey(
                        name: "FK_ShippingCosts_Weight_Units_Weight_UnitId",
                        column: x => x.Weight_UnitId,
                        principalTable: "Weight_Units",
                        principalColumn: "Weight_UnitId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Logistic_shippingType_LogisticId",
                table: "Logistic_shippingType",
                column: "LogisticId");

            migrationBuilder.CreateIndex(
                name: "IX_Logistic_shippingType_ShippingTypeId",
                table: "Logistic_shippingType",
                column: "ShippingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Logistics_Business_id",
                table: "Logistics",
                column: "Business_id");

            migrationBuilder.CreateIndex(
                name: "IX_Logistics_CurrencyId",
                table: "Logistics",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingCosts_Business_id",
                table: "ShippingCosts",
                column: "Business_id");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingCosts_Weight_UnitId",
                table: "ShippingCosts",
                column: "Weight_UnitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logistic_shippingType");

            migrationBuilder.DropTable(
                name: "ShippingCosts");

            migrationBuilder.DropTable(
                name: "Logistics");

            migrationBuilder.DropTable(
                name: "ShippingTypes");

            migrationBuilder.DropTable(
                name: "Weight_Units");
        }
    }
}
