using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace pp_bot.Server.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Achievements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BotUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: true),
                    TelegramId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    ChatId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.ChatId);
                });

            migrationBuilder.CreateTable(
                name: "BotUserChat",
                columns: table => new
                {
                    ChatUsersId = table.Column<int>(type: "integer", nullable: false),
                    UserChatsChatId = table.Column<long>(type: "bigint", nullable: false),
                    PPLength = table.Column<int>(type: "integer", nullable: false),
                    LastManipulationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotUserChat", x => new { x.ChatUsersId, x.UserChatsChatId });
                    table.ForeignKey(
                        name: "FK_BotUserChat_BotUsers_ChatUsersId",
                        column: x => x.ChatUsersId,
                        principalTable: "BotUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BotUserChat_Chats_UserChatsChatId",
                        column: x => x.UserChatsChatId,
                        principalTable: "Chats",
                        principalColumn: "ChatId",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateTable(
                name: "GrowHistories",
                columns: table => new
                {
                    GrowHistoryId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PPLengthChange = table.Column<int>(type: "integer", nullable: false),
                    ExecutionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    BotUserChatChatUsersId = table.Column<int>(type: "integer", nullable: true),
                    BotUserChatUserChatsChatId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrowHistories", x => x.GrowHistoryId);
                    table.ForeignKey(
                        name: "FK_GrowHistories_BotUserChat_BotUserChatChatUsersId_BotUserCha~",
                        columns: x => new { x.BotUserChatChatUsersId, x.BotUserChatUserChatsChatId },
                        principalTable: "BotUserChat",
                        principalColumns: new[] { "ChatUsersId", "UserChatsChatId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AchievementBotUserChat_UsersAcquiredChatUsersId_UsersAcquir~",
                table: "AchievementBotUserChat",
                columns: new[] { "UsersAcquiredChatUsersId", "UsersAcquiredUserChatsChatId" });

            migrationBuilder.CreateIndex(
                name: "IX_BotUserChat_UserChatsChatId",
                table: "BotUserChat",
                column: "UserChatsChatId");

            migrationBuilder.CreateIndex(
                name: "IX_GrowHistories_BotUserChatChatUsersId_BotUserChatUserChatsCh~",
                table: "GrowHistories",
                columns: new[] { "BotUserChatChatUsersId", "BotUserChatUserChatsChatId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AchievementBotUserChat");

            migrationBuilder.DropTable(
                name: "GrowHistories");

            migrationBuilder.DropTable(
                name: "Achievements");

            migrationBuilder.DropTable(
                name: "BotUserChat");

            migrationBuilder.DropTable(
                name: "BotUsers");

            migrationBuilder.DropTable(
                name: "Chats");
        }
    }
}
