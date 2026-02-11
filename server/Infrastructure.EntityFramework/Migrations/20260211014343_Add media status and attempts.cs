using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class Addmediastatusandattempts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "Media");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Media",
                type: "text",
                nullable: false,
                defaultValue: "pending");

            migrationBuilder.AddColumn<int>(
                name: "UploadAttempts",
                table: "Media",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "UploadAttempts",
                table: "Media");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Media",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Media",
                type: "text",
                nullable: true);
        }
    }
}
