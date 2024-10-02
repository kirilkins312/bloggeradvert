using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Instadvert.CZ.Migrations
{
    /// <inheritdoc />
    public partial class CompUpd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BloggerUser_Url",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BloggerUser_Url",
                table: "AspNetUsers");
        }
    }
}
