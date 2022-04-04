using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace pp_bot.Server.Migrations
{
    public partial class ReworkDbStructure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AchievementBotUserChat");

            migrationBuilder.DropTable(
                name: "GrowHistories");

            migrationBuilder.DropTable(
                name: "BotUserChat");

            migrationBuilder.DropTable(
                name: "BotUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Chats",
                table: "Chats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Achievements",
                table: "Achievements");

            migrationBuilder.RenameTable(
                name: "Chats",
                newName: "chats");

            migrationBuilder.RenameTable(
                name: "Achievements",
                newName: "achievements");

            migrationBuilder.RenameColumn(
                name: "ChatId",
                table: "chats",
                newName: "chat_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "achievements",
                newName: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_chats",
                table: "chats",
                column: "chat_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_achievements",
                table: "achievements",
                column: "id");

            migrationBuilder.CreateTable(
                name: "bot_users",
                columns: table => new
                {
                    telegram_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    username = table.Column<string>(type: "text", nullable: true),
                    display_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bot_users", x => x.telegram_id);
                });

            migrationBuilder.CreateTable(
                name: "bot_user__chat",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    chat_id = table.Column<long>(type: "bigint", nullable: false),
                    pp_length = table.Column<int>(type: "integer", nullable: false),
                    utc_updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bot_user__chat", x => x.id);
                    table.UniqueConstraint("AK_bot_user__chat_chat_id_user_id", x => new { x.chat_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_bot_user__chat_bot_users_user_id",
                        column: x => x.user_id,
                        principalTable: "bot_users",
                        principalColumn: "telegram_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_bot_user__chat_chats_chat_id",
                        column: x => x.chat_id,
                        principalTable: "chats",
                        principalColumn: "chat_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "bot_user_to_chat__achievement",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    achievement__id = table.Column<int>(type: "integer", nullable: false),
                    bot_user_to_chat__id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bot_user_to_chat__achievement", x => x.id);
                    table.UniqueConstraint("AK_bot_user_to_chat__achievement_achievement__id_bot_user_to_c~", x => new { x.achievement__id, x.bot_user_to_chat__id });
                    table.ForeignKey(
                        name: "FK_bot_user_to_chat__achievement_achievements_achievement__id",
                        column: x => x.achievement__id,
                        principalTable: "achievements",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_bot_user_to_chat__achievement_bot_user__chat_bot_user_to_ch~",
                        column: x => x.bot_user_to_chat__id,
                        principalTable: "bot_user__chat",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "grow_history",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    pp_length_change = table.Column<int>(type: "integer", nullable: false),
                    utc_created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grow_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_grow_history_bot_user__chat_id",
                        column: x => x.id,
                        principalTable: "bot_user__chat",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_bot_user__chat_user_id",
                table: "bot_user__chat",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_bot_user_to_chat__achievement_bot_user_to_chat__id",
                table: "bot_user_to_chat__achievement",
                column: "bot_user_to_chat__id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bot_user_to_chat__achievement");

            migrationBuilder.DropTable(
                name: "grow_history");

            migrationBuilder.DropTable(
                name: "bot_user__chat");

            migrationBuilder.DropTable(
                name: "bot_users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_chats",
                table: "chats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_achievements",
                table: "achievements");

            migrationBuilder.RenameTable(
                name: "chats",
                newName: "Chats");

            migrationBuilder.RenameTable(
                name: "achievements",
                newName: "Achievements");

            migrationBuilder.RenameColumn(
                name: "chat_id",
                table: "Chats",
                newName: "ChatId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Achievements",
                newName: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Chats",
                table: "Chats",
                column: "ChatId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Achievements",
                table: "Achievements",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "BotUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                    TelegramId = table.Column<long>(type: "bigint", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BotUserChat",
                columns: table => new
                {
                    ChatUsersId = table.Column<int>(type: "integer", nullable: false),
                    UserChatsChatId = table.Column<long>(type: "bigint", nullable: false),
                    LastManipulationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PPLength = table.Column<int>(type: "integer", nullable: false)
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
                    BotUserChatChatUsersId = table.Column<int>(type: "integer", nullable: true),
                    BotUserChatUserChatsChatId = table.Column<long>(type: "bigint", nullable: true),
                    ExecutionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PPLengthChange = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrowHistories", x => x.GrowHistoryId);
                    table.ForeignKey(
                        name: "FK_GrowHistories_BotUserChat_BotUserChatChatUsersId_BotUserCha~",
                        columns: x => new { x.BotUserChatChatUsersId, x.BotUserChatUserChatsChatId },
                        principalTable: "BotUserChat",
                        principalColumns: new[] { "ChatUsersId", "UserChatsChatId" });
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
    }
}
