using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Instadvert.CZ.Migrations
{
    /// <inheritdoc />
    public partial class PhoneNumUpd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhoneNumberPrefix",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumberPrefix",
                table: "AspNetUsers");
        }
    }
}
