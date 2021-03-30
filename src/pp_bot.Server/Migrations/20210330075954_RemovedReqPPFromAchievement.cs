using Microsoft.EntityFrameworkCore.Migrations;

namespace pp_bot.Server.Migrations
{
    public partial class RemovedReqPPFromAchievement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequiredPPLength",
                table: "Achievements");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "RequiredPPLength",
                table: "Achievements",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
