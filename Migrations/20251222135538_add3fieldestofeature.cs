using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Universal_server.Migrations
{
    /// <inheritdoc />
    public partial class add3fieldestofeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Insert_by",
                table: "Features",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "Insert_on",
                table: "Features",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<bool>(
                name: "visible",
                table: "Features",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Insert_by",
                table: "Features");

            migrationBuilder.DropColumn(
                name: "Insert_on",
                table: "Features");

            migrationBuilder.DropColumn(
                name: "visible",
                table: "Features");
        }
    }
}
