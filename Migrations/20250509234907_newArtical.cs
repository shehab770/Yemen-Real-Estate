using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webProgramming.Migrations
{
    /// <inheritdoc />
    public partial class newArtical : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "Articles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NeighbId",
                table: "Articles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "NeighbId",
                table: "Articles");
        }
    }
}
