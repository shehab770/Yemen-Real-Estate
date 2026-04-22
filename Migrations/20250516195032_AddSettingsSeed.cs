using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webProgramming.Migrations
{
    /// <inheritdoc />
    public partial class AddSettingsSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AboutUs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AboutUs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiteAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SiteEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SitePhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LogoImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FooterLogoImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SocialMediaLink = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CopyrightText = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Id", "CopyrightText", "FooterLogoImage", "LogoImage", "SiteAddress", "SiteEmail", "SitePhone", "SocialMediaLink" },
                values: new object[] { 1, "© 2024 Real Estate Platform. All rights reserved.", "/images/default-footer-logo.png", "/images/default-logo.png", "123 Real Estate Street, New York, NY 10001", "contact@realestate.com", "(555) 123-4567", "{\"facebook\":\"https://facebook.com\",\"twitter\":\"https://twitter.com\",\"instagram\":\"https://instagram.com\",\"whatsapp\":\"https://wa.me\"}" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AboutUs");

            migrationBuilder.DropTable(
                name: "Settings");
        }
    }
}
