using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webProgramming.Migrations
{
    /// <inheritdoc />
    public partial class MessageMigrationUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IsRead",
                table: "Messages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "Messages");
        }
    }
}
