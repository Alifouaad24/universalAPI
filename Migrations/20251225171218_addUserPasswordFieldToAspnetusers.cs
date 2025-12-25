using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class addUserPasswordFieldToAspnetusers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserPassword",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserPassword",
                table: "AspNetUsers");
        }
    }
}
