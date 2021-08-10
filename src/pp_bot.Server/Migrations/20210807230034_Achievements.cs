using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace pp_bot.Server.Migrations
{
    public partial class Achievements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChatName",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "LastManipulationTime",
                table: "BotUsers");

            migrationBuilder.DropColumn(
                name: "PPLength",
                table: "BotUsers");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastManipulationTime",
                table: "BotUserChat",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "PPLength",
                table: "BotUserChat",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Achievements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ImageFileName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AchievementBotUserChat",
                columns: table => new
                {
                    AcquiredAchievementsId = table.Column<int>(type: "integer", nullable: false),
                    UsersAcquiredChatUsersId = table.Column<int>(type: "integer", nullable: false),
                    UsersAcquiredUserChatsChatId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AchievementBotUserChat", x => new { x.AcquiredAchievementsId, x.UsersAcquiredChatUsersId, x.UsersAcquiredUserChatsChatId });
                    table.ForeignKey(
                        name: "FK_AchievementBotUserChat_Achievements_AcquiredAchievementsId",
                        column: x => x.AcquiredAchievementsId,
                        principalTable: "Achievements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AchievementBotUserChat_BotUserChat_UsersAcquiredChatUsersId~",
                        columns: x => new { x.UsersAcquiredChatUsersId, x.UsersAcquiredUserChatsChatId },
                        principalTable: "BotUserChat",
                        principalColumns: new[] { "ChatUsersId", "UserChatsChatId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AchievementBotUserChat_UsersAcquiredChatUsersId_UsersAcquir~",
                table: "AchievementBotUserChat",
                columns: new[] { "UsersAcquiredChatUsersId", "UsersAcquiredUserChatsChatId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AchievementBotUserChat");

            migrationBuilder.DropTable(
                name: "Achievements");

            migrationBuilder.DropColumn(
                name: "LastManipulationTime",
                table: "BotUserChat");

            migrationBuilder.DropColumn(
                name: "PPLength",
                table: "BotUserChat");

            migrationBuilder.AddColumn<string>(
                name: "ChatName",
                table: "Chats",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastManipulationTime",
                table: "BotUsers",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "PPLength",
                table: "BotUsers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
