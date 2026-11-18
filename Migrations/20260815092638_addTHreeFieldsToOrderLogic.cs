using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addTHreeFieldsToOrderLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConsumerBusiness_id",
                table: "GlobalOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentityUserDataId",
                table: "GlobalOrders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentityUserDataId",
                table: "GlobalOrderDetails",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GlobalOrders_ConsumerBusiness_id",
                table: "GlobalOrders",
                column: "ConsumerBusiness_id");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalOrders_IdentityUserDataId",
                table: "GlobalOrders",
                column: "IdentityUserDataId");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalOrderDetails_IdentityUserDataId",
                table: "GlobalOrderDetails",
                column: "IdentityUserDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalOrderDetails_AspNetUsers_IdentityUserDataId",
                table: "GlobalOrderDetails",
                column: "IdentityUserDataId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalOrders_AspNetUsers_IdentityUserDataId",
                table: "GlobalOrders",
                column: "IdentityUserDataId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalOrders_Businesses_ConsumerBusiness_id",
                table: "GlobalOrders",
                column: "ConsumerBusiness_id",
                principalTable: "Businesses",
                principalColumn: "Business_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GlobalOrderDetails_AspNetUsers_IdentityUserDataId",
                table: "GlobalOrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_GlobalOrders_AspNetUsers_IdentityUserDataId",
                table: "GlobalOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_GlobalOrders_Businesses_ConsumerBusiness_id",
                table: "GlobalOrders");

            migrationBuilder.DropIndex(
                name: "IX_GlobalOrders_ConsumerBusiness_id",
                table: "GlobalOrders");

            migrationBuilder.DropIndex(
                name: "IX_GlobalOrders_IdentityUserDataId",
                table: "GlobalOrders");

            migrationBuilder.DropIndex(
                name: "IX_GlobalOrderDetails_IdentityUserDataId",
                table: "GlobalOrderDetails");

            migrationBuilder.DropColumn(
                name: "ConsumerBusiness_id",
                table: "GlobalOrders");

            migrationBuilder.DropColumn(
                name: "IdentityUserDataId",
                table: "GlobalOrders");

            migrationBuilder.DropColumn(
                name: "IdentityUserDataId",
                table: "GlobalOrderDetails");
        }
    }
}
