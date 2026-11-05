using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace xema.Migrations
{
    /// <inheritdoc />
    public partial class RemoveImageUrlFromEverywhere : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "TattooImages");

            migrationBuilder.AlterColumn<int>(
                name: "MediaId",
                table: "TattooImages",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "MediaId",
                table: "TattooImages",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "TattooImages",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
