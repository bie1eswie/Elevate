using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elevate.Data.Migrations.HumanAPI
{
    /// <inheritdoc />
    public partial class public_token : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PublicToken",
                table: "HumanAPIUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicToken",
                table: "HumanAPIUsers");
        }
    }
}
