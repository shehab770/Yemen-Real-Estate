using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webProgramming.Migrations
{
    /// <inheritdoc />
    public partial class newestArtical : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HouseType",
                table: "Articles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HouseType",
                table: "Articles");
        }
    }
}
