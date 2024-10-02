using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Instadvert.CZ.Migrations
{
    /// <inheritdoc />
    public partial class StripeAcc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "StripeAccActivated",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "StripeAccCreated",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "StripeReqDataFilled",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StripeAccActivated",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StripeAccCreated",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StripeReqDataFilled",
                table: "AspNetUsers");
        }
    }
}
