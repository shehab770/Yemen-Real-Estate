using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webProgramming.Migrations
{
    /// <inheritdoc />
    public partial class chatAndMessageMigrationUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChatId",
                table: "Chats",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "Chats");
        }
    }
}
