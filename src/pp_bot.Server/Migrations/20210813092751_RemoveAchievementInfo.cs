using Microsoft.EntityFrameworkCore.Migrations;

namespace pp_bot.Server.Migrations
{
    public partial class RemoveAchievementInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Achievements");

            migrationBuilder.DropColumn(
                name: "ImageFileName",
                table: "Achievements");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Achievements");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Achievements",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageFileName",
                table: "Achievements",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Achievements",
                type: "text",
                nullable: true);
        }
    }
}
