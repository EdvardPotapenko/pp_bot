using Microsoft.EntityFrameworkCore.Migrations;

namespace pp_bot.Server.Migrations
{
    public partial class LastLengthChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LastPPLengthChange",
                table: "BotUserChat",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastPPLengthChange",
                table: "BotUserChat");
        }
    }
}
