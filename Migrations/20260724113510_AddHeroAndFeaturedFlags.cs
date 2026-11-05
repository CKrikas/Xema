using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace xema.Migrations
{
    /// <inheritdoc />
    public partial class AddHeroAndFeaturedFlags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "TattooImages",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHeroImage",
                table: "TattooImages",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "TattooImages");

            migrationBuilder.DropColumn(
                name: "IsHeroImage",
                table: "TattooImages");
        }
    }
}
