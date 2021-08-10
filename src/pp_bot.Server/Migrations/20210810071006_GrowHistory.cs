using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace pp_bot.Server.Migrations
{
    public partial class GrowHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastPPLengthChange",
                table: "BotUserChat");

            migrationBuilder.CreateTable(
                name: "GrowHistories",
                columns: table => new
                {
                    GrowHistoryId = table.Column<long>(type: "bigint", nullable: false),
                    UserChatId = table.Column<int>(type: "integer", nullable: false),
                    PPLengthChange = table.Column<int>(type: "integer", nullable: false),
                    ExecutionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    BotUserChatChatUsersId = table.Column<int>(type: "integer", nullable: true),
                    BotUserChatUserChatsChatId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrowHistories", x => new { x.GrowHistoryId, x.UserChatId });
                    table.ForeignKey(
                        name: "FK_GrowHistories_BotUserChat_BotUserChatChatUsersId_BotUserCha~",
                        columns: x => new { x.BotUserChatChatUsersId, x.BotUserChatUserChatsChatId },
                        principalTable: "BotUserChat",
                        principalColumns: new[] { "ChatUsersId", "UserChatsChatId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GrowHistories_BotUserChatChatUsersId_BotUserChatUserChatsCh~",
                table: "GrowHistories",
                columns: new[] { "BotUserChatChatUsersId", "BotUserChatUserChatsChatId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GrowHistories");

            migrationBuilder.AddColumn<int>(
                name: "LastPPLengthChange",
                table: "BotUserChat",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
