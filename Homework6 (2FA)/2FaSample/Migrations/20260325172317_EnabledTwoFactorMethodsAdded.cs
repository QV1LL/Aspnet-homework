using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2FaSample.Migrations
{
    /// <inheritdoc />
    public partial class EnabledTwoFactorMethodsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreferredTwoFactorMethod",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "EnabledTwoFactorMethods",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnabledTwoFactorMethods",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "PreferredTwoFactorMethod",
                table: "AspNetUsers",
                type: "TEXT",
                maxLength: 10,
                nullable: true);
        }
    }
}
