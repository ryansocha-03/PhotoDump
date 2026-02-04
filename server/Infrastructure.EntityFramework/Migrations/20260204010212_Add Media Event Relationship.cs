using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddMediaEventRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "Media",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "Media",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PublicFileName",
                table: "Media",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Media_EventId",
                table: "Media",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Media_Events_EventId",
                table: "Media",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Media_Events_EventId",
                table: "Media");

            migrationBuilder.DropIndex(
                name: "IX_Media_EventId",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "PublicFileName",
                table: "Media");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "Media",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
